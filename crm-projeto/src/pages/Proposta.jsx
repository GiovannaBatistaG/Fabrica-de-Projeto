import React, { useState, useEffect } from "react";
import api from "../services/api";
import aiApi from "../services/aiApi";
import { jsPDF } from "jspdf";
import { useLocation } from "react-router-dom";
import "./Proposta.css";

const PropostaPage = () => {
  const [leadId, setLeadId] = useState("");
  const [promptExtra, setPromptExtra] = useState("");
  const [plano, setPlano] = useState("simples");
  const [faixaValor, setFaixaValor] = useState("500-1000");
  const [servicosOferecidos, setServicosOferecidos] = useState([]);
  const [slides, setSlides] = useState("");
  const [leadsList, setLeadsList] = useState([]);
  const [loading, setLoading] = useState(false);
  const [erro, setErro] = useState("");
  const [pdfUrl, setPdfUrl] = useState(null);
  const [validado, setValidado] = useState(false);
  const [leadData, setLeadData] = useState(null);
  const [mostrarCustomizacao, setMostrarCustomizacao] = useState(false);
  const [corPrimaria, setCorPrimaria] = useState("#667eea");
  const [corSecundaria, setCorSecundaria] = useState("#ffffff");
  const [tamanhoFonte, setTamanhoFonte] = useState("normal");

  const servicosDisponiveis = [
    { id: 'suporte', label: 'Suporte Humanizado' },
    { id: 'identidade', label: 'Identidade Visual' },
    { id: 'interface', label: 'Criação de Interface' },
    { id: 'reuniao', label: 'Reunião para Alinhamento' },
    { id: 'contrato', label: 'Contrato com 2 Reajustes' },
    { id: 'fidelidade', label: 'Fidelidade' },
  ];

  const location = useLocation();

  useEffect(() => {
    if (location && location.state && location.state.leadId) {
      setLeadId(String(location.state.leadId));
    }

  }, []);

  useEffect(() => {
    const fetchLeads = async () => {
      try {
        const res = await api.get('/api/Clientes');
        setLeadsList(res.data || []);
      } catch (e) {
        console.warn('Não foi possível buscar lista de leads', e);
      }
    };
    fetchLeads();
  }, []);

  useEffect(() => {
    return () => {
      if (pdfUrl) URL.revokeObjectURL(pdfUrl);
    };
  }, [pdfUrl]);

  const toggleServico = (id) => {
    setServicosOferecidos((prev) =>
      prev.includes(id) ? prev.filter((s) => s !== id) : [...prev, id]
    );
  };

  const gerarSlides = async () => {
    setLoading(true);
    setErro("");
    setSlides("");
    setPdfUrl(null);
    setValidado(false);

    try {

      const leadResponse = await api.get(`/api/Clientes/${leadId}`); 
      const lead = leadResponse.data;
      setLeadData(lead);

      const iaResponse = await aiApi.post(`/gerar-proposta`, {
        ...lead,
        promptExtra: promptExtra.trim(),
        plano,
        faixaValor,
        servicosOferecidos,
      });

      const generatedSlides = iaResponse.data.slides;
      setSlides(generatedSlides);
      if (iaResponse.data.pdfUrl) {
        if (pdfUrl) URL.revokeObjectURL(pdfUrl);
        setPdfUrl(iaResponse.data.pdfUrl);
      } else {
        const url = await gerarPdfLocal(lead, generatedSlides);
        if (pdfUrl) URL.revokeObjectURL(pdfUrl);
        setPdfUrl(url);
      }
    } catch (err) {
      console.error("Erro:", err);
      setErro(
        "Falha ao gerar slides. Verifique se o ID existe e o servidor está rodando."
      );
    } finally {
      setLoading(false);
    }
  };

  const mandarParaValidacao = async () => {
    if (!leadId || !leadData) return alert('Selecione um lead e gere a proposta antes de enviar.');
    if (!slides && !pdfUrl) return alert('Gere a proposta antes de enviar para validação.');

    try {
      const clienteId = leadId;

      const propostaPayload = { 
        slides, 
        pdfUrl, 
        statusValidacao: "Aguardando Validação" 
      };

      const responseProposta = await api.post(`/api/propostas-cliente/${clienteId}/criar`, propostaPayload);
      
      alert('Proposta enviada para validação com sucesso! Ela aparecerá na página de Validar.');

    } catch (e) {
      console.error('Erro completo ao enviar proposta para validação:', e);
      const errorMessage = e?.response?.data?.message || e?.response?.data?.detail || e?.message || 'Erro desconhecido';
      alert(`Falha ao enviar proposta para validação: ${errorMessage}`);
    }
  };


  const toDataURL = (blob) =>
    new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onloadend = () => resolve(reader.result);
      reader.onerror = reject;
      reader.readAsDataURL(blob);
    });

  const gerarPdfLocal = async (lead, slidesText) => {
    try {
      const doc = new jsPDF({ 
        orientation: "landscape", 
        unit: "mm", 
        format: [254, 142.875]
      });
      
      const pageWidth = doc.internal.pageSize.getWidth();
      const pageHeight = doc.internal.pageSize.getHeight();
      const margin = 20;
      
      const hexToRgb = (hex) => {
        const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
        return result ? [
          parseInt(result[1], 16),
          parseInt(result[2], 16),
          parseInt(result[3], 16)
        ] : [255, 255, 255];
      };
      
      const getNomeServicos = () => {
        const mapaServicos = {
          'suporte': 'Suporte Humanizado',
          'identidade': 'Identidade Visual',
          'interface': 'Criação de Interface',
          'reuniao': 'Reunião para Alinhamento',
          'contrato': 'Contrato com 2 Reajustes',
          'fidelidade': 'Fidelidade',
        };
        return servicosOferecidos.map(id => mapaServicos[id]).join(', ');
      };

      const addSlide = (title, content, bgColor = '#ffffff', textColor = '#1f2937') => {
        doc.setFillColor(...hexToRgb(bgColor));
        doc.rect(0, 0, pageWidth, pageHeight, 'F');

        doc.setDrawColor(...hexToRgb('#667eea'));
        doc.setLineWidth(2);
        doc.line(0, 0, pageWidth, 0);

        doc.setFontSize(32);
        doc.setTextColor(...hexToRgb(textColor));
        doc.setFont('helvetica', 'bold');
        const titleLines = doc.splitTextToSize(title, pageWidth - margin * 2);
        let y = margin + 15;
        titleLines.forEach((line) => {
          doc.text(line, margin, y);
          y += 12;
        });

        doc.setDrawColor(...hexToRgb('#667eea'));
        doc.setLineWidth(1);
        doc.line(margin, y + 5, pageWidth - margin, y + 5);

        doc.setFontSize(14);
        doc.setFont('helvetica', 'normal');
        doc.setTextColor(...hexToRgb(textColor));
        y += 15;
        
        if (Array.isArray(content)) {
          content.forEach((line) => {
            if (y > pageHeight - margin) {
              doc.addPage();
              doc.setFillColor(...hexToRgb(bgColor));
              doc.rect(0, 0, pageWidth, pageHeight, 'F');
              y = margin;
            }
            doc.text(line, margin + 5, y);
            y += 8;
          });
        } else {
          const contentLines = doc.splitTextToSize(content, pageWidth - margin * 2 - 10);
          contentLines.forEach((line) => {
            if (y > pageHeight - margin) {
              doc.addPage();
              doc.setFillColor(...hexToRgb(bgColor));
              doc.rect(0, 0, pageWidth, pageHeight, 'F');
              y = margin;
            }
            doc.text(line, margin + 5, y);
            y += 8;
          });
        }
      };

      doc.setFillColor(102, 126, 234);
      doc.rect(0, 0, pageWidth, pageHeight, 'F');
      
      doc.setFontSize(48);
      doc.setTextColor(255, 255, 255);
      doc.setFont('helvetica', 'bold');
      doc.text('PROPOSTA COMERCIAL', pageWidth / 2, pageHeight / 2 - 15, { align: 'center' });
      
      doc.setFontSize(24);
      doc.setFont('helvetica', 'normal');
      doc.text(`Cliente: ${lead?.nome || '-'}`, pageWidth / 2, pageHeight / 2 + 10, { align: 'center' });
      
      doc.setFontSize(16);
      doc.setTextColor(200, 200, 255);
      const dataAtual = new Date().toLocaleDateString('pt-BR');
      doc.text(`${dataAtual}`, pageWidth / 2, pageHeight / 2 + 25, { align: 'center' });

      doc.addPage();

      const clienteContent = [
        `Cliente: ${lead?.nome || '-'}`,
        `Email: ${lead?.email || '-'}`,
        `Telefone: ${lead?.telefone || '-'}`,
        `Mensagem: ${lead?.mensagem || '-'}`,
      ];
      addSlide('Informações do Cliente', clienteContent, '#ffffff', '#1f2937');
      doc.addPage();

      const planoLabel = plano.charAt(0).toUpperCase() + plano.slice(1);
      const detalhesContent = [
        `Tipo de Plano: ${planoLabel}`,
        `Faixa de Valor: ${faixaValor}`,
        ...((servicosOferecidos.length > 0) ? [`Serviços: ${getNomeServicos()}`] : []),
        ...((promptExtra.trim()) ? [`Observações: ${promptExtra}`] : []),
      ];
      addSlide('Detalhes da Proposta', detalhesContent, '#ffffff', '#1f2937');
      doc.addPage();

      const sections = slidesText.split('\n\n').map(s => s.trim()).filter(Boolean);
      const colors = ['#ffffff', '#ffffff', '#ffffff', '#ffffff', '#ffffff', '#ffffff'];
      let colorIndex = 0;

      sections.forEach((section) => {
        const lines = section.split('\n');
        const header = lines[0];
        const body = lines.slice(1).join('\n');
        
        const bgColor = colors[colorIndex % colors.length];
        addSlide(header, body, bgColor, '#1f2937');
        
        doc.addPage();
        colorIndex++;
      });

      const blob = doc.output('blob');
      const url = URL.createObjectURL(blob);
      return url;
    } catch (e) {
      console.error('Erro ao gerar PDF local:', e);
      return null;
    }
  };

  return (
    <div className="proposta-container">
      <div className="proposta-controls-wrapper">
        <div className="proposta-section-header">
          <h2> Proposta</h2>
        </div>

        <div className="proposta-controls">
          <div className="proposta-select-group">
            <label>Selecionar Cliente</label>
            <select
              value={leadId}
              onChange={async (e) => {
                const id = e.target.value;
                setLeadId(id);
                if (id) {
                  try {
                    const res = await api.get(`/api/Clientes/${id}`); 
                    setLeadData(res.data);
                  } catch (err) {
                    console.warn('Erro ao buscar lead selecionado', err);
                  }
                } else {
                  setLeadData(null);
                }
              }}
            >
              <option value="">Selecione</option>
              {leadsList.map((l) => (
                <option key={l.id} value={l.id}>{`${l.id} - ${l.nome}`}</option> 
              ))}
            </select>
          </div>

          <div className="proposta-prompt-group">
            <label>Mensagem de Retorno</label>
            <input
              value={promptExtra}
              onChange={(e) => setPromptExtra(e.target.value)}
              placeholder="Descreva o retorno desejado para a proposta..."
            />
          </div>
        </div>

        <div className="proposta-form-row">
          <div className="proposta-select-group">
            <label>Tipo de Plano</label>
            <select
              value={plano}
              onChange={(e) => setPlano(e.target.value)}
            >
              <option value="simples">Simples</option>
              <option value="intermediario">Intermediário</option>
              <option value="premium">Premium</option>
            </select>
          </div>

          <div className="proposta-select-group">
            <label>Faixa de Valor</label>
            <select
              value={faixaValor}
              onChange={(e) => setFaixaValor(e.target.value)}
            >
              <option value="500-1000">R$ 500 - R$ 1.000</option>
              <option value="1000-2000">R$ 1.000 - R$ 2.000</option>
              <option value="2000-5000">R$ 2.000 - R$ 5.000</option>
            </select>
          </div>
        </div>

        <div className="proposta-servicos-section">
          <label className="proposta-servicos-label">Serviços a Oferecer</label>
          <div className="proposta-servicos-grid">
            {servicosDisponiveis.map((servico) => (
              <div key={servico.id} className="proposta-servico-item">
                <input
                  type="checkbox"
                  id={servico.id}
                  checked={servicosOferecidos.includes(servico.id)}
                  onChange={() => toggleServico(servico.id)}
                />
                <label htmlFor={servico.id}>{servico.label}</label>
              </div>
            ))}
          </div>
        </div>

        <button
          onClick={gerarSlides}
          disabled={!leadId || loading}
          className="proposta-button-gerar"
        >
          {loading ? "Gerando..." : "Gerar Proposta"}
        </button>
      </div>

      {erro && (
        <p className="proposta-erro">{erro}</p>
      )}

      {mostrarCustomizacao && (
        <div className="proposta-customizacao-modal">
          <div className="proposta-customizacao-conteudo">
            <h2>Personalizar PDF</h2>
            
            <div className="proposta-customizacao-grupo">
              <label htmlFor="corPrimaria">Cor Primária</label>
              <input
                type="color"
                id="corPrimaria"
                value={corPrimaria}
                onChange={(e) => setCorPrimaria(e.target.value)}
              />
            </div>

            <div className="proposta-customizacao-grupo">
              <label htmlFor="corSecundaria">Cor Secundária (Fundo)</label>
              <input
                type="color"
                id="corSecundaria"
                value={corSecundaria}
                onChange={(e) => setCorSecundaria(e.target.value)}
              />
            </div>

            <div className="proposta-customizacao-grupo">
              <label htmlFor="tamanhoFonte">Tamanho da Fonte</label>
              <select
                id="tamanhoFonte"
                value={tamanhoFonte}
                onChange={(e) => setTamanhoFonte(e.target.value)}
              >
                <option value="pequeno">Pequeno (12px)</option>
                <option value="normal">Normal (14px)</option>
                <option value="grande">Grande (16px)</option>
                <option value="extra">Extra Grande (18px)</option>
              </select>
            </div>

            <div className="proposta-customizacao-botoes">
              <button
                onClick={() => setMostrarCustomizacao(false)}
                className="proposta-customizacao-confirmar"
              >
                ✓ Aplicar
              </button>
              <button
                onClick={() => {
                  setMostrarCustomizacao(false);
                  setCorPrimaria("#667eea");
                  setCorSecundaria("#ffffff");
                  setTamanhoFonte("normal");
                }}
                className="proposta-customizacao-cancelar"
              >
                ✕ Cancelar
              </button>
            </div>
          </div>
        </div>
      )}

      {slides && (
        <div className="proposta-resultado">
          <h2>Slides Gerados:</h2>
          <pre className="proposta-slides-preview">{slides}</pre>

          {pdfUrl && (
            <div className="proposta-pdf-section">
              <h3>Visualização da Proposta (PDF)</h3>
              <button
                onClick={() => setMostrarCustomizacao(true)}
                style={{
                  marginBottom: '10px',
                  padding: '8px 16px',
                  backgroundColor: '#667eea',
                  color: 'white',
                  border: 'none',
                  borderRadius: '6px',
                  cursor: 'pointer',
                  fontWeight: '600',
                  fontSize: '12px',
                }}
              >
                Personalizar Cores e Fontes
              </button>
              <iframe
                src={pdfUrl}
                title="Visualização da proposta"
                className="proposta-pdf-iframe"
              ></iframe>

              <div className="proposta-buttons-group">
                <button
                  onClick={mandarParaValidacao}
                  className="proposta-button-validacao"
                >
                  Mandar para Validação
                </button>

                <button
                  onClick={gerarSlides}
                  className="proposta-button-gerar-novamente"
                >
                  Gerar Novamente
                </button>

                <button
                  onClick={() => {
                    try {
                      if (leadData && slides) {
                        const doc = new jsPDF({ 
                          orientation: "landscape", 
                          unit: "mm", 
                          format: [254, 142.875]
                        });
                        
                        const pageWidth = doc.internal.pageSize.getWidth();
                        const pageHeight = doc.internal.pageSize.getHeight();
                        const margin = 20;

                        const hexToRgb = (hex) => {
                          const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
                          return result ? [
                            parseInt(result[1], 16),
                            parseInt(result[2], 16),
                            parseInt(result[3], 16)
                          ] : [255, 255, 255];
                        };

                        const addSlide = (title, content, bgColor = '#ffffff', textColor = '#1f2937') => {
                          doc.setFillColor(...hexToRgb(bgColor));
                          doc.rect(0, 0, pageWidth, pageHeight, 'F');

                          doc.setDrawColor(...hexToRgb('#667eea'));
                          doc.setLineWidth(2);
                          doc.line(0, 0, pageWidth, 0);

                          doc.setFontSize(32);
                          doc.setTextColor(...hexToRgb(textColor));
                          doc.setFont('helvetica', 'bold');
                          const titleLines = doc.splitTextToSize(title, pageWidth - margin * 2);
                          let y = margin + 15;
                          titleLines.forEach((line) => {
                            doc.text(line, margin, y);
                            y += 12;
                          });

                          doc.setDrawColor(...hexToRgb('#667eea'));
                          doc.setLineWidth(1);
                          doc.line(margin, y + 5, pageWidth - margin, y + 5);

                          doc.setFontSize(14);
                          doc.setFont('helvetica', 'normal');
                          doc.setTextColor(...hexToRgb(textColor));
                          y += 15;
                          
                          if (Array.isArray(content)) {
                            content.forEach((line) => {
                              if (y > pageHeight - margin) {
                                doc.addPage();
                                doc.setFillColor(...hexToRgb(bgColor));
                                doc.rect(0, 0, pageWidth, pageHeight, 'F');
                                y = margin;
                              }
                              doc.text(line, margin + 5, y);
                              y += 8;
                            });
                          } else {
                            const contentLines = doc.splitTextToSize(content, pageWidth - margin * 2 - 10);
                            contentLines.forEach((line) => {
                              if (y > pageHeight - margin) {
                                doc.addPage();
                                doc.setFillColor(...hexToRgb(bgColor));
                                doc.rect(0, 0, pageWidth, pageHeight, 'F');
                                y = margin;
                              }
                              doc.text(line, margin + 5, y);
                              y += 8;
                            });
                          }
                        };

                        doc.setFillColor(102, 126, 234);
                        doc.rect(0, 0, pageWidth, pageHeight, 'F');
                        
                        doc.setFontSize(48);
                        doc.setTextColor(255, 255, 255);
                        doc.setFont('helvetica', 'bold');
                        doc.text('PROPOSTA COMERCIAL', pageWidth / 2, pageHeight / 2 - 15, { align: 'center' });
                        
                        doc.setFontSize(24);
                        doc.setFont('helvetica', 'normal');
                        doc.text(`Cliente: ${leadData.nome || '-'}`, pageWidth / 2, pageHeight / 2 + 10, { align: 'center' });
                        
                        doc.setFontSize(16);
                        doc.setTextColor(200, 200, 255);
                        const dataAtual = new Date().toLocaleDateString('pt-BR');
                        doc.text(`${dataAtual}`, pageWidth / 2, pageHeight / 2 + 25, { align: 'center' });

                        doc.addPage();

                        const clienteContent = [
                          `Cliente: ${leadData.nome || '-'}`,
                          `Email: ${leadData.email || '-'}`,
                          `Telefone: ${leadData.telefone || '-'}`,
                          `Mensagem: ${leadData.mensagem || '-'}`,
                        ];
                        addSlide('Informações do Cliente', clienteContent, '#ffffff', '#1f2937');
                        doc.addPage();

                        const planoLabel = plano.charAt(0).toUpperCase() + plano.slice(1);
                        const mapaServicos = {
                          'suporte': 'Suporte Humanizado',
                          'identidade': 'Identidade Visual',
                          'interface': 'Criação de Interface',
                          'reuniao': 'Reunião para Alinhamento',
                          'contrato': 'Contrato com 2 Reajustes',
                          'fidelidade': 'Fidelidade',
                        };
                        const nomeServicos = servicosOferecidos.map(id => mapaServicos[id]).join(', ');
                        const detalhesContent = [
                          `Tipo de Plano: ${planoLabel}`,
                          `Faixa de Valor: ${faixaValor}`,
                          ...((servicosOferecidos.length > 0) ? [`Serviços: ${nomeServicos}`] : []),
                          ...((promptExtra.trim()) ? [`Observações: ${promptExtra}`] : []),
                        ];
                        addSlide('Detalhes da Proposta', detalhesContent, '#ffffff', '#1f2937');
                        doc.addPage();

                        const sections = slides.split('\n\n').map(s => s.trim()).filter(Boolean);
                        const colors = ['#f3e5f5', '#e3f2fd', '#f1f8e9', '#fce4ec', '#ede7f6', '#e0f2f1'];
                        let colorIndex = 0;

                        sections.forEach((section) => {
                          const lines = section.split('\n');
                          const header = lines[0];
                          const body = lines.slice(1).join('\n');
                          
                          const bgColor = colors[colorIndex % colors.length];
                          addSlide(header, body, bgColor, '#1f2937');
                          
                          doc.addPage();
                          colorIndex++;
                        });

                        doc.save(`proposta_${leadData.nome || leadId}.pdf`);
                      } else if (pdfUrl) {
                        fetch(pdfUrl)
                          .then((r) => r.blob())
                          .then((blob) => {
                            const link = document.createElement('a');
                            link.href = URL.createObjectURL(blob);
                            link.download = `proposta_${leadId}.pdf`;
                            document.body.appendChild(link);
                            link.click();
                            link.remove();
                          });
                      }
                    } catch (e) {
                      console.error('Erro ao baixar PDF:', e);
                    }
                  }}
                >
                  Baixar PDF
                </button>
              </div>

            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default PropostaPage;
