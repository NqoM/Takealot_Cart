using Microsoft.Playwright;

namespace Takealot_Cart
{
    [TestFixture]
    public class Tests
    {
        private IPage page;

        [OneTimeSetUp]
        public async Task InitBrowser()
        {
            //Open Takealot Home Page
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                Channel = "chrome"
            });

            page = await browser.NewPageAsync();
            await page.GotoAsync("https://www.takealot.com/");
        }


        [Test, Order(10)]
        [TestCase("Magneto LED Lantern 1000 lumen - 2.0")]
        [TestCase("Wahl Cutek Professional Dryer  2000 Watt")]
        public async Task TakeAlot_SearchProductByUsingTitle_ThenAddTheProductToCart(string productTitle)
        {
            // Define Search Box
            var searchBox = page.Locator(PageElements.SearchBox);
            await searchBox.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });

            //Assert if it's visible and enabled
            Assert.IsTrue(await searchBox.IsEnabledAsync());

            //Search for product
            await searchBox.FillAsync(productTitle);

            //Define Search Button
            var searchBtn = page.Locator(PageElements.SearchButton);
            await searchBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });

            //Assert if it's visible and enabled
            Assert.IsTrue(await searchBtn.IsEnabledAsync());

            await searchBtn.ClickAsync();

            //Define Result Grid
            var resultGrid = page.Locator(PageElements.SearchResults);
            await resultGrid.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });

            //Assert if it's visible and enabled
            Assert.IsTrue(await resultGrid.IsEnabledAsync());

            //Find item that has Exact title
            var product = resultGrid.Filter(new()
            {
                HasText = productTitle
            }).First;

            //Assert if it's visible and enabled
            Assert.IsTrue(await product.IsEnabledAsync());

            //Add to cart
            var addToCartBtn = product.Locator(PageElements.AddToCartButton).First;
            await addToCartBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });

            //Assert if it's visible and enabled
            Assert.IsTrue(await addToCartBtn.IsEnabledAsync());

            await addToCartBtn.ScrollIntoViewIfNeededAsync();
            await addToCartBtn.ClickAsync();

            //Assert Added To Cart Button
            var addedToCartBtn = product.Locator(PageElements.AddedToCartButton);
            await addedToCartBtn.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });

            //Assert if it's visible and enabled
            Assert.IsTrue(await addedToCartBtn.IsEnabledAsync());
        }

        [Test, Order(20)]
        public async Task TakeAlot_OpenShoppingCart()
        {
            // Open shopping cart
            var shoppingCart = page.Locator(PageElements.ShoppingCart);
            await shoppingCart.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });

            Assert.IsTrue(await shoppingCart.IsEnabledAsync());
            await shoppingCart.ClickAsync();

            Assert.AreEqual(page.Url, "https://www.takealot.com/cart");
        }

        [Test, Order(30)]
        [TestCase("Wahl Cutek Professional Dryer  2000 Watt")]
        [TestCase("Magneto LED Lantern 1000 lumen - 2.0")]
        public async Task TakeAlot_VerifyProductsInShoppingCart(string productTitle)
        {

            // Verify products
            var shoppingList = page.Locator(PageElements.ShoppingList);
            var product = shoppingList.GetByRole(AriaRole.Heading, new() { Name = productTitle }).First;
            await product.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 10000 });

            Assert.AreEqual(productTitle, await product.TextContentAsync());
        }
    }
}