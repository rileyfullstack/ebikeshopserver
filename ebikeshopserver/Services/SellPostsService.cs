using System;
using ebikeshopserver.Exceptions;
using ebikeshopserver.Models.SellPost;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ebikeshopserver.Services
{
	public class SellPostsService
	{
        private IMongoCollection<SellPost> _sellPosts;

        public SellPostsService(IMongoClient mongoClient)
		{
            var database = mongoClient.GetDatabase("ebike_shop_server");
            _sellPosts = database.GetCollection<SellPost>("sellPosts");
        }

        public async Task<SellPost> CreateSellPostAsync(SellPost newSellPost) //Add functionality 
        {
            var existingPost = await _sellPosts.Find(u => u.Title == newSellPost.Title).FirstOrDefaultAsync();
            if(existingPost != null)
            {
                throw new PostAlreadyExistsException("A post with this title already exists.");
            }
            await _sellPosts.InsertOneAsync(newSellPost);
            return newSellPost;
        }

        public async Task<List<SellPost>> GetAllSellPostsAsync() //Only non deleted
        {
            var allSalePosts = await _sellPosts.Find(_ => true).ToListAsync();
            if(allSalePosts == null)
            {
                throw new NoPostsFoundException("No posts have been found (Possible DB connection issue).");
            }
            return allSalePosts;
        }

        public async Task<List<SellPost>> GetSellPostsAsync() //Only non deleted
        {
            var allSalePosts = await _sellPosts.Find(p => p.Status != SellPostStatus.Deleted).ToListAsync();
            if (allSalePosts == null)
            {
                throw new NoPostsFoundException("No posts have been found (Possible DB connection issue).");
            }
            return allSalePosts;
        }

        //Don't send subcatagory to only search using a catagory, otherwise send both.
        public async Task<List<SellPost>> GetCatagorySellPosts(string catagory, string? subcatagory = null)
        {
            var catagorySalePosts = await _sellPosts.Find(s => s.Catagory == catagory && subcatagory!=null ? s.SubCatagory==subcatagory : true).ToListAsync();
            if (catagorySalePosts == null)
                throw new NoPostsFoundException("No posts have been found in this catagoty/subcatagory.");
            return catagorySalePosts;
        }

        public async Task<SellPost> GetSellPostAsync(string postId)
        {
            var salePost = await _sellPosts.Find(s => s._id.ToString() == postId).FirstOrDefaultAsync();
            if(salePost == null)
            {
                throw new NoPostsFoundException($"No post with the ID {postId} have been found.");
            }
            return salePost;
        }

        //This will be used to alter any details about a post.
        //No matter if you only need to change one thing, like the price, or the status, it will all be used here.
        public async Task<SellPost> EditSellPostAsync(string postId, SellPost updatedSellPost) 
        {
            var filter = Builders<SellPost>.Filter.Eq(sp => sp._id, new ObjectId(postId));

            var update = Builders<SellPost>.Update
                .Set(sp => sp.Title, updatedSellPost.Title)
                .Set(sp => sp.Description, updatedSellPost.Description)
                .Set(sp => sp.Catagory, updatedSellPost.Catagory)
                .Set(sp => sp.SubCatagory, updatedSellPost.SubCatagory)
                .Set(sp => sp.CatagoryId, updatedSellPost.CatagoryId)
                .Set(sp => sp.SubcatagoryId, updatedSellPost.SubcatagoryId)
                .Set(sp => sp.Price, updatedSellPost.Price)
                .Set(sp => sp.Image, updatedSellPost.Image)
                .Set(sp => sp.CurrentDiscount, updatedSellPost.CurrentDiscount)
                .Set(sp => sp.Specifications, updatedSellPost.Specifications)
                .Set(sp => sp.ItemsLeft, updatedSellPost.ItemsLeft)
                .Set(sp => sp.Status, updatedSellPost.Status)
                .Set(sp => sp.LastUpdatedDate, DateTime.Now);

            var result = await _sellPosts.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                throw new NoPostsFoundException(postId.ToString());
            }

            return updatedSellPost; 
        }

        public async Task<List<SellPost>> GetSellPostsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            var filter = Builders<SellPost>.Filter.Where(sp => sp.Price >= minPrice && sp.Price <= maxPrice && sp.Status != SellPostStatus.Deleted);
            var sellPostsInRange = await _sellPosts.Find(filter).ToListAsync();

            if (sellPostsInRange == null || sellPostsInRange.Count == 0)
            {
                throw new NoPostsFoundException("No posts found in the specified price range.");
            }

            return sellPostsInRange;
        }

        public async Task<List<SellPost>> GetSellPostsBySellerId(string sellerId)
        {
            var filter = Builders<SellPost>.Filter.Eq(sp => sp.SellerId, sellerId);
            var sellPosts = await _sellPosts.Find(filter).ToListAsync();

            if (sellPosts == null || sellPosts.Count == 0)
            {
                throw new NoPostsFoundException($"No posts found for the seller with ID {sellerId}.");
            }

            return sellPosts;
        }

    }
}

