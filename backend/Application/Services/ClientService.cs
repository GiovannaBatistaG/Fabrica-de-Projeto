using ProjetoApi.Application.DTOs;
using ProjetoApi.Application.Ports.Input;
using ProjetoApiPT.Domain.Entities;
using ProjetoApiPT.Domain.Ports;

namespace ProjetoApi.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IClienteRepositorio _clienteRepositorio;

        public ClientService(IClienteRepositorio clienteRepositorio)
        {
            _clienteRepositorio = clienteRepositorio;
        }

        public async Task<ClientDTO?> GetByIdAsync(int id)
        {
            var cliente = await _clienteRepositorio.ObterPorIdAsync(id);
            return cliente != null ? MapToDTO(cliente) : null;
        }

        public async Task<PagedResponseDTO<ClientDTO>> GetPaginatedAsync(int page, int pageSize)
        {
            var clientes = await _clienteRepositorio.ObterTodosAsync();
            var clientesList = clientes.ToList();

            var totalItems = clientesList.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var skip = (page - 1) * pageSize;

            var pagedClientes = clientesList
                .Skip(skip)
                .Take(pageSize)
                .Select(MapToDTO)
                .ToList();

            return new PagedResponseDTO<ClientDTO>
            {
                Items = pagedClientes,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalCount = totalItems
            };
        }

        public async Task<ClientDTO> CreateAsync(ClientDTO clientDTO)
        {
            var cliente = MapToEntity(clientDTO);
            var createdCliente = await _clienteRepositorio.CriarAsync(cliente);
            return MapToDTO(createdCliente);
        }

        public async Task<ClientDTO?> UpdateAsync(int id, ClientDTO clientDTO)
        {
            var existingCliente = await _clienteRepositorio.ObterPorIdAsync(id);
            if (existingCliente == null)
                return null;

            // Mantém os valores que não devem ser atualizados pelo cliente
            clientDTO.Id = id;
            var cliente = MapToEntity(clientDTO);
            cliente.DataCadastro = existingCliente.DataCadastro;
            
            await _clienteRepositorio.AtualizarAsync(cliente);
            return MapToDTO(cliente);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cliente = await _clienteRepositorio.ObterPorIdAsync(id);
            if (cliente == null)
                return false;

            await _clienteRepositorio.ExcluirAsync(id);
            return true;
        }

        public async Task<bool> UpdatePdfStatusAsync(int id, bool pdfGerado)
        {
            var cliente = await _clienteRepositorio.ObterPorIdAsync(id);
            if (cliente == null)
                return false;

            cliente.PdfGerado = pdfGerado;
            await _clienteRepositorio.AtualizarAsync(cliente);
            return true;
        }

        private static ClientDTO MapToDTO(Cliente cliente)
        {
            return new ClientDTO
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email,
                Mensagem = cliente.Mensagem,
                Telefone = cliente.Telefone,
                PdfGerado = cliente.PdfGerado,
                DataCadastro = cliente.DataCadastro
            };
        }

        private static Cliente MapToEntity(ClientDTO dto)
        {
            return new Cliente
            {
                Id = dto.Id,
                Nome = dto.Nome,
                Email = dto.Email,
                Mensagem = dto.Mensagem,
                Telefone = dto.Telefone,
                PdfGerado = dto.PdfGerado,
                DataCadastro = dto.DataCadastro
            };
        }
    }
}
