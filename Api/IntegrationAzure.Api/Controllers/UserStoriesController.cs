using Microsoft.AspNetCore.Mvc;
using IntegrationAzure.Api.Application.Services;
using IntegrationAzure.Api.Application.DTOs;
using FluentValidation;

namespace IntegrationAzure.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserStoriesController : BaseController
    {
        private readonly UserStoryService _userStoryService;
        private readonly IValidator<CreateUserStoryDto>? _createValidator;
        private readonly IValidator<UpdateUserStoryDto>? _updateValidator;

        public UserStoriesController(
            UserStoryService userStoryService,
            IValidator<CreateUserStoryDto>? createValidator = null,
            IValidator<UpdateUserStoryDto>? updateValidator = null)
        {
            _userStoryService = userStoryService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Obtém todas as histórias de usuário
        /// </summary>
        /// <returns>Lista de histórias de usuário</returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<List<UserStorySummaryDto>>>> GetAll()
        {
            try
            {
                var result = await _userStoryService.GetAllAsync();
                return ProcessServiceResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse<List<UserStorySummaryDto>>(
                    "Erro interno do servidor",
                    new List<string> { ex.Message },
                    500
                );
            }
        }

        /// <summary>
        /// Obtém uma história de usuário por ID
        /// </summary>
        /// <param name="id">ID da história de usuário</param>
        /// <returns>História de usuário encontrada</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<UserStoryDto>>> GetById(Guid id)
        {
            try
            {
                var result = await _userStoryService.GetByIdAsync(id);
                return ProcessServiceResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse<UserStoryDto>(
                    "Erro interno do servidor",
                    new List<string> { ex.Message },
                    500
                );
            }
        }

        /// <summary>
        /// Cria uma nova história de usuário
        /// </summary>
        /// <param name="createDto">Dados da história de usuário a ser criada</param>
        /// <returns>História de usuário criada</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<UserStoryDto>>> Create([FromBody] CreateUserStoryDto createDto)
        {
            try
            {
                // Validação manual se o validador estiver disponível
                if (_createValidator != null)
                {
                    var validationResult = await _createValidator.ValidateAsync(createDto);
                    if (!validationResult.IsValid)
                    {
                        var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                        return ErrorResponse<UserStoryDto>(
                            "Dados de entrada inválidos",
                            errors,
                            400
                        );
                    }
                }

                var currentUser = GetCurrentUser();
                var result = await _userStoryService.CreateAsync(createDto, currentUser);

                if (result.Success && result.Data != null)
                {
                    return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
                }

                return ProcessServiceResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse<UserStoryDto>(
                    "Erro interno do servidor",
                    new List<string> { ex.Message },
                    500
                );
            }
        }

        /// <summary>
        /// Atualiza uma história de usuário existente
        /// </summary>
        /// <param name="id">ID da história de usuário</param>
        /// <param name="updateDto">Dados atualizados da história de usuário</param>
        /// <returns>História de usuário atualizada</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<UserStoryDto>>> Update(Guid id, [FromBody] UpdateUserStoryDto updateDto)
        {
            try
            {
                // Validação manual se o validador estiver disponível
                if (_updateValidator != null)
                {
                    var validationResult = await _updateValidator.ValidateAsync(updateDto);
                    if (!validationResult.IsValid)
                    {
                        var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                        return ErrorResponse<UserStoryDto>(
                            "Dados de entrada inválidos",
                            errors,
                            400
                        );
                    }
                }

                var currentUser = GetCurrentUser();
                var result = await _userStoryService.UpdateAsync(id, updateDto, currentUser);
                return ProcessServiceResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse<UserStoryDto>(
                    "Erro interno do servidor",
                    new List<string> { ex.Message },
                    500
                );
            }
        }

        /// <summary>
        /// Exclui uma história de usuário
        /// </summary>
        /// <param name="id">ID da história de usuário</param>
        /// <returns>Confirmação da exclusão</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> Delete(Guid id)
        {
            try
            {
                var result = await _userStoryService.DeleteAsync(id);
                return ProcessServiceResponse(result);
            }
            catch (Exception ex)
            {
                return ErrorResponse<bool>(
                    "Erro interno do servidor",
                    new List<string> { ex.Message },
                    500
                );
            }
        }
    }
}