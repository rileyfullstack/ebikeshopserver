using System;
using ebikeshopserver.Models.SellPosts;

namespace ebikeshopserver.Authorization
{
    public class AuthorizationService
    {
        public bool AuthorizeSellPostAccess(string sellerId, SellPost sellPost)
        {
            return sellPost.SellerId == sellerId;
        }
    }

}

