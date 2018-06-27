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
    [RequirePermissionAttribute]
    [Route("api/v1/comments")]
    public class AlgoStoreCommentsController : Controller
    {
        private readonly IAlgoStoreCommentsService _commentsService;

        public AlgoStoreCommentsController(IAlgoStoreCommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        [HttpGet("algoComments")]
        [SwaggerOperation("GetAllCommentsForAlgoAsync")]
        [DescriptionAttribute("Gives users the ability to see all available comments for an algo")]
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
        [SwaggerOperation("GetCommentById")]
        [DescriptionAttribute("Allows users to see a single comment by a given ID")]
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
        [SwaggerOperation("CreateComment")]
        [DescriptionAttribute("Allows the creation of comments")]
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
        [SwaggerOperation("EditComment")]
        [DescriptionAttribute("Allows the editing of an existing comment")]
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
        [SwaggerOperation("DeleteComment")]
        [DescriptionAttribute("Allows the deletion of a specific comment")]
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
