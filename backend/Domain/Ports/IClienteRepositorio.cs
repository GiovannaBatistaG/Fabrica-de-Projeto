using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjetoApiPT.Domain.Entities;

namespace ProjetoApiPT.Domain.Ports
{
    /// <summary>
    /// Define o contrato para operações de persistência de dados (CRUD) da entidade Cliente.
    /// Esta interface segue o padrão Repository e permite a abstração do acesso aos dados.
    /// </summary>
    public interface IClienteRepositorio
    {
        /// <summary>
        /// Adiciona um novo Cliente ao repositório.
        /// </summary>
        /// <param name="cliente">O objeto Cliente a ser criado. Não pode ser nulo.</param>
        /// <returns>O Cliente criado, incluindo seu Id gerado pelo banco de dados.</returns>
        Task<Cliente> CriarAsync(Cliente cliente);

        /// <summary>
        /// Retorna todos os Clientes do repositório.
        /// </summary>
        /// <returns>Uma coleção de todos os Clientes cadastrados. Pode retornar uma lista vazia.</returns>
        Task<IEnumerable<Cliente>> ObterTodosAsync();

        /// <summary>
        /// Retorna um Cliente pelo seu identificador único.
        /// </summary>
        /// <param name="id">O Id do Cliente a ser recuperado.</param>
        /// <returns>O Cliente correspondente ou null se não for encontrado.</returns>
        Task<Cliente?> ObterPorIdAsync(int id);

        /// <summary>
        /// Atualiza as informações de um Cliente existente.
        /// </summary>
        /// <param name="cliente">O objeto Cliente com os dados atualizados. Não pode ser nulo.</param>
        Task AtualizarAsync(Cliente cliente);

        /// <summary>
        /// Remove um Cliente do repositório pelo seu identificador.
        /// </summary>
        /// <param name="id">O Id do Cliente a ser removido.</param>
        Task ExcluirAsync(int id);
    }
}