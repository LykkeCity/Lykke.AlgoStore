using AutoMapper;
using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
    [Route("api/v1/comments")]
    public class AlgoStoreCommentsController: Controller
    {
        private readonly IAlgoStoreCommentsService _commentsService;

        public AlgoStoreCommentsController(IAlgoStoreCommentsService commentsService)
        { 
            _commentsService = commentsService;
        }
        
        [HttpGet("algoComments")]
        [RequirePermission]
        [SwaggerOperation("GetAllCommentsForAlgoAsync")]
        [ProducesResponseType(typeof(List<AlgoCommentModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllCommentsForAlgoAsync(string algoId)
        {
            var clientId = User.GetClientId();

            var result = await _commentsService.GetAlgoCommentsAsync(algoId, clientId);

            var response = Mapper.Map<List<AlgoCommentModel>>(result);

            return Ok(response);
        }

        [HttpGet("getCommentById")]
        [RequirePermission]
        [SwaggerOperation("GetCommentById")]
        [ProducesResponseType(typeof(AlgoCommentModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetCommentById(string algoId, string commentId)
        {
            var clientId = User.GetClientId();

            var result = await _commentsService.GetCommentByIdAsync(algoId, commentId, clientId);

            if (result == null)
                return NotFound();

            var response = Mapper.Map<AlgoCommentModel>(result);

            return Ok(response);
        }

        [HttpPost("algoComments")]
        [RequirePermission]
        [SwaggerOperation("CreateComment")]
        [ProducesResponseType(typeof(AlgoCommentModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CreateComment([FromBody] AlgoCommentModel model)
        {
            var data = Mapper.Map<AlgoCommentData>(model);
            data.Author = User.GetClientId();

            var result = await _commentsService.SaveCommentAsync(data);

            var response = Mapper.Map<AlgoCommentModel>(result);

            return Ok(response);
        }

        [HttpPatch("algoComments")]
        [RequirePermission]
        [SwaggerOperation("EditComment")]
        [ProducesResponseType(typeof(AlgoCommentModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> EditComment([FromBody] AlgoCommentModel model)
        {
            var data = Mapper.Map<AlgoCommentData>(model);
            data.Author = User.GetClientId();

            var result = await _commentsService.EditCommentAsync(data);

            var response = Mapper.Map<AlgoCommentModel>(result);

            return Ok(response);
        }

        [HttpDelete("algoComments")]
        [RequirePermission]
        [SwaggerOperation("DeleteComment")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteComment(string algoId, string commentId)
        {
            var clientId = User.GetClientId();

            await _commentsService.DeleteCommentAsync(algoId, commentId, clientId);

            return NoContent();
        }
    }
}
