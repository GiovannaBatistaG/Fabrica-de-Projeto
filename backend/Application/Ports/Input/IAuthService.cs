using ProjetoApi.Application.DTOs;

namespace ProjetoApi.Application.Ports.Input
{
    public interface IAuthService
    {
        /// <summary>
        /// Realiza o login do usuário
        /// </summary>
        /// <param name="loginDTO">Dados de login do usuário</param>
        /// <returns>Resultado do login contendo o token e informações do usuário</returns>
        Task<LoginResultDTO> LoginAsync(LoginDTO loginDTO);

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        /// <param name="registerDTO">Dados de registro do novo usuário</param>
        /// <returns>Resultado do registro contendo o token e informações do usuário</returns>
        Task<LoginResultDTO> RegisterAsync(RegisterDTO registerDTO);
    }
}
