using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SetupChrome
    {
        public async Task<RemoteWebDriver> GetRemoveWebDriver()
        {
#if DEBUG
            RemoteWebDriver driver = new RemoteWebDriver(new Uri("http://localhost:4444"), GetOptionsForHeadlessChrome().ToCapabilities(), TimeSpan.FromMinutes(3));
#else
            RemoteWebDriver driver = new RemoteWebDriver(new Uri("http://localhost:4444"), GetOptionsForHeadlessChrome().ToCapabilities(), TimeSpan.FromMinutes(5));
#endif

#if DEBUG
            await Task.Delay(1000);
#else
            await Task.Delay(7500);
#endif
            return driver;
        }

        public ChromeOptions GetOptionsForHeadlessChrome()
        {
            ChromeOptions chromeOptions = new ChromeOptions();

            chromeOptions.AddArgument("--disable-extensions");
            chromeOptions.AddArgument("--disable-gpu");
            //chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--disable-notifications");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("--mute-audio");
            chromeOptions.AddArgument("--no-first-run");
            chromeOptions.AddArgument("--no-initial-navigation");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--blink-settings=imagesEnabled=false");
            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--disable-browser-side-navigation");
            chromeOptions.AddArgument("--window-size=1280,1024");
            chromeOptions.AddArguments("--user-agent=Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36");
            return chromeOptions;
        }
    }
}
