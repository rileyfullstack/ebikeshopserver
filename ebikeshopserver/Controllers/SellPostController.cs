using System;
using ebikeshopserver.Exceptions;
using ebikeshopserver.Models.SellPost;
using ebikeshopserver.Models.User;
using ebikeshopserver.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ebikeshopserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellPostController : ControllerBase
    {
		private SellPostsService _sellPostsService;

		public SellPostController(IMongoClient mongoClient)
		{
			_sellPostsService = new SellPostsService(mongoClient);
		}

        [HttpGet]
        public async Task<IActionResult> GetAllSellPosts() //No authoraty needed
        {
            List<SellPost> result = await _sellPostsService.GetSellPostsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSpecificSellPost(string id) 
        {
            try
            {
                SellPost sellPost = await _sellPostsService.GetSellPostAsync(id);
                return Ok(sellPost);
            }
            catch (NoPostsFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetCatagorySellPosts(string category)
        {
            try
            {
                var sellPosts = await _sellPostsService.GetCatagorySellPosts(category);
                return Ok(sellPosts);
            }
            catch (NoPostsFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("posts")]
        public async Task<IActionResult> GetSellPostsByCategory([FromQuery] string category, [FromQuery] string? subcategory = null)
        {
            try
            {
                var sellPosts = await _sellPostsService.GetCatagorySellPosts(category, subcategory);
                return Ok(sellPosts);
            }
            catch (NoPostsFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        public async Task<IActionResult> DeleteSellPost(string postId) //Make sure its only available to admins or sellers who created the post.
        {
            try
            {
                SellPost existingPost = await _sellPostsService.GetSellPostAsync(postId);

                // Update the status to Deleted
                existingPost.Status = SellPostStatus.Deleted;
                await _sellPostsService.EditSellPostAsync(postId, existingPost);
                return Ok($"Post with ID {postId} has been marked as deleted.");
            }
            catch (NoPostsFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSellPost([FromBody] SellPost sellPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string postId = sellPost.PostId.ToString();
                SellPost updatedPost = await _sellPostsService.EditSellPostAsync(postId, sellPost);
                return Ok(updatedPost);
            }
            catch (NoPostsFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

