﻿using BlazorEcommerce.Server.Services.AuthService;
using Stripe;
using Stripe.Checkout;

namespace BlazorEcommerce.Server.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly ICartService _cartService;
        private readonly IAuthService _authService;
        private readonly IOrderService _orderService;

        const string secret = "whsec_6935881fdecbe21ec1e841f4e36bf2f56ad303aa7604584c9ae7ab0994445433";

        public PaymentService(ICartService cartService,
            IAuthService authService,
            IOrderService orderService)
        {
            //StripeConfiguration.ApiKey = "sk_test_51HnFFuJWja1dketA1LY3VQds3XWpByD5GE8laKrxyNldWKnXXdktvITJiG3PYNDMwpSkrAv33d7JjvHDEUGPPo2E00vkDMlVIb";
            StripeConfiguration.SetApiKey("sk_test_51Ngi5DJx1JWrUoJb8gBHdfiG7nLaAkzmBoDX7eGzjyhglcH0ykJiqzSblleDjSACIVhpCLivqpTvVmbs6Fp4Mh7U00fnY9Kw2r");
            _cartService = cartService;
            _authService = authService;
            _orderService = orderService;
        }

        public async Task<Session> CreateCheckoutSession()
        {
            var products = (await _cartService.GetDbCartProducts()).Data;
            var lineItems = new List<SessionLineItemOptions>();
            products.ForEach(product => lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = product.Price * 100,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = product.Title,
                        Images = new List<string> { product.ImageUrl }
                    }
                },
                Quantity = product.Quantity
            }));

            var options = new SessionCreateOptions
            {
                CustomerEmail = _authService.GetUserEmail(),
                ShippingAddressCollection =
                    new SessionShippingAddressCollectionOptions
                    {
                        AllowedCountries = new List<string> { "US" }
                    },
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = @"https://localhost:7211/order-success",
                CancelUrl = @"https://localhost:7211/cart"
            };

            var service = new SessionService();
            Session session = service.Create(options);
            return session;
        }

        public async Task<ServiceResponse<bool>> FulfillOrder(HttpRequest request)
        {
            var json = await new StreamReader(request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                        json,
                        request.Headers["Stripe-Signature"],
                        secret,
                        throwOnApiVersionMismatch: false
                    );

                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;
                    var user = await _authService.GetUserByEmail(session.CustomerEmail);
                    await _orderService.PlaceOrder(user.Id);
                }

                return new ServiceResponse<bool> { Data = true };
            }
            catch (StripeException e)
            {
                return new ServiceResponse<bool> { Data = false, Success = false, Message = e.Message };
            }
        }
    }
}