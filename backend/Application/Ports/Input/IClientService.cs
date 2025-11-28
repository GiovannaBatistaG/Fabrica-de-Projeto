using ProjetoApi.Application.DTOs;

namespace ProjetoApi.Application.Ports.Input
{
    public interface IClientService
    {
        /// <summary>
        /// Obtém um cliente pelo seu ID
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <returns>DTO do cliente encontrado ou null</returns>
        Task<ClientDTO?> GetByIdAsync(int id);

        /// <summary>
        /// Obtém uma lista paginada de clientes
        /// </summary>
        /// <param name="page">Número da página (começa em 1)</param>
        /// <param name="pageSize">Quantidade de itens por página</param>
        /// <returns>Lista paginada de clientes</returns>
        Task<PagedResponseDTO<ClientDTO>> GetPaginatedAsync(int page, int pageSize);

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="clientDTO">Dados do cliente a ser criado</param>
        /// <returns>DTO do cliente criado</returns>
        Task<ClientDTO> CreateAsync(ClientDTO clientDTO);

        /// <summary>
        /// Atualiza um cliente existente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <param name="clientDTO">Novos dados do cliente</param>
        /// <returns>DTO do cliente atualizado ou null se não encontrado</returns>
        Task<ClientDTO?> UpdateAsync(int id, ClientDTO clientDTO);

        /// <summary>
        /// Exclui um cliente pelo seu ID
        /// </summary>
        /// <param name="id">ID do cliente a ser excluído</param>
        /// <returns>True se o cliente foi excluído, False se não foi encontrado</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Atualiza o status de geração do PDF de um cliente
        /// </summary>
        /// <param name="id">ID do cliente</param>
        /// <param name="pdfGerado">Novo status de geração do PDF</param>
        /// <returns>True se atualizado com sucesso, False se o cliente não foi encontrado</returns>
        Task<bool> UpdatePdfStatusAsync(int id, bool pdfGerado);
    }
}
