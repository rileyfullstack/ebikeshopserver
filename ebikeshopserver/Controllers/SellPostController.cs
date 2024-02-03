using System;
using ebikeshopserver.Exceptions;
using ebikeshopserver.Models.SellPosts;
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
        public async Task<IActionResult> GetAllSellPosts() 
        {
            List<SellPost> result = await _sellPostsService.GetSellPostsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSellPost(string id) 
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

        [HttpPost]
        public async Task<IActionResult> CreateSellPost([FromBody] SellPost newSellPost) //Allow only to sellers and admins, as well as make sure to take the token from the sellers to use in the id of the new post.
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                SellPost createdPost = await _sellPostsService.CreateSellPostAsync(newSellPost);
                return CreatedAtAction(nameof(GetSellPost), new { id = createdPost._id.ToString() }, createdPost);
            }
            catch (PostAlreadyExistsException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeleteSellPost(string postId) //Make sure its only available to admins or sellers who created the post.
        {
            Console.WriteLine("Test");
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

        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdateSellPost([FromBody] SellPost sellPost, string postId) //Make sure to only make available to sellers who posted it, as well as admins.
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
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

        [HttpGet("filter/price")]
        public async Task<IActionResult> GetSellPostsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            try
            {
                var sellPosts = await _sellPostsService.GetSellPostsByPriceRange(minPrice, maxPrice);
                return Ok(sellPosts);
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

        [HttpGet("seller/{sellerId}")]
        public async Task<IActionResult> GetSellPostsBySellerId(string sellerId) 
        {
            try
            {
                var sellPosts = await _sellPostsService.GetSellPostsBySellerId(sellerId);
                return Ok(sellPosts);
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