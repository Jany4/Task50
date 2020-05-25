using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace Task50
{
    public class Tests
    {
        IWebDriver _driver;

        //explicit waiter for the webelement
        bool wait(IWebDriver driver, By locator)
        {
            WebDriverWait waiter = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            waiter.PollingInterval = TimeSpan.FromMilliseconds(100); //changing polling frequency

            try
            {
                var element = waiter.Until(condition =>
                {
                    try
                    {
                        var elementToBeDisplayed = _driver.FindElement(locator);
                        return elementToBeDisplayed.Displayed;
                    }
                    catch (NoSuchElementException)
                    {
                        return false;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return false;
                    }

                });

                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        // making screenshot, saving to the file in jpeg format
        void MakeScreenShot(IWebDriver driver, string path)
        {
            Screenshot img = ((ITakesScreenshot)driver).GetScreenshot();
            img.SaveAsFile(path, ScreenshotImageFormat.Png);
        }

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
        }

        [TearDown]
        public void CleanUp()
        {
            _driver.Close();
        }

        [TestCase("seleniumtests@tut.by", "123456789zxcvbn")]
        [TestCase("seleniumtests2@tut.by", "123456789zxcvbn")]

        [Test]
        public void LoginTest(string loginstr, string passwordstr)
        {
            _driver.Navigate().GoToUrl("https://mail.tut.by");

            MakeScreenShot(_driver, "/Users/macbookpro/Desktop/TestScreenshot.png"); //making screenshot, saving it to the Desktop

            IWebElement loginField = _driver.FindElement(By.Id("Username"));
            IWebElement passwrdField = _driver.FindElement(By.Id("Password"));
            IWebElement loginButton = _driver.FindElement(By.XPath("//input[contains(@class, \"loginButton\")]"));

            loginField.SendKeys(loginstr);
            passwrdField.SendKeys(passwordstr);
            loginButton.Click();

         //   Thread.Sleep(10000); //neither explicit, nor implicit waiter. Implicit waiters work for all the elements. And the explicit waiters works for a particular element with the particular condition. Thread.Sleep() just stop the thread with executing code for a particular time.

           


            Assert.IsTrue(wait(_driver, By.XPath("//div[@class=\"mail-User-Name\"]")));
           
        }

        [Test]
        public void MultiSelectTest()
        {
            _driver.Navigate().GoToUrl("https://www.seleniumeasy.com/test/basic-select-dropdown-demo.html");

            SelectElement selectedSates = new SelectElement(_driver.FindElement(By.XPath("//select[@id = \"multi-select\"]"))); //add check for multiselect enabled

            Random rnd = new Random();
            
            for (int i =0; i < 3; i++)
            {
                selectedSates.SelectByIndex(rnd.Next(1, 8));
            }

            bool result = true;
            for (int i = 0; i < 3; i++)
            {
                result = selectedSates.AllSelectedOptions[i].Selected;
            }

            Assert.IsTrue(result);
        }

        [Test]
        public void ConfirmOkAlertTest()
        {
            _driver.Navigate().GoToUrl("https://www.seleniumeasy.com/test/javascript-alert-box-demo.html");
            _driver.FindElement(By.XPath("//button[@onclick=\"myConfirmFunction()\"]")).Click();

            try
            {
                IAlert confirmAlert = _driver.SwitchTo().Alert();
                confirmAlert.Accept();

                IWebElement resultMessage = _driver.FindElement(By.Id("confirm-demo"));
                Assert.AreEqual(resultMessage.Text, "You pressed OK!");
              
            }
            catch(NoAlertPresentException)
            {
                Assert.Fail("No Alert was displayed");
            }
            finally
            {
                _driver.Close();
            }
            
        }


        [Test]
        public void ConfirmCancelAlertTest()
        {
            _driver.Navigate().GoToUrl("https://www.seleniumeasy.com/test/javascript-alert-box-demo.html");
            _driver.FindElement(By.XPath("//button[@onclick=\"myConfirmFunction()\"]")).Click();

            try
            {
                IAlert confirmAlert = _driver.SwitchTo().Alert();
                confirmAlert.Dismiss();

                IWebElement resultMessage = _driver.FindElement(By.Id("confirm-demo"));
                Assert.AreEqual(resultMessage.Text, "You pressed Cancel!");

            }
            catch (NoAlertPresentException)
            {
                Assert.Fail("No Alert was displayed");
            }
            finally
            {
                _driver.Close();
            }

        }

        [TestCase("Test Name", "Test Name")]
        [Test]
        public void AlertBoxTest(string inValue, string outValue)
        {
            _driver.Navigate().GoToUrl("https://www.seleniumeasy.com/test/javascript-alert-box-demo.html");
            _driver.FindElement(By.XPath("//button[@onclick=\"myPromptFunction()\"]")).Click();

            try
            {
                IAlert confirmAlert = _driver.SwitchTo().Alert();
                confirmAlert.SendKeys(inValue);
                confirmAlert.Accept();

                IWebElement resultMessage = _driver.FindElement(By.Id("prompt-demo"));
                Assert.AreEqual(resultMessage.Text, $"You have entered '{outValue}' !", "Actual value is not equal to the entered text");

            }
            catch (NoAlertPresentException)
            {
                Assert.Fail("No Alert was displayed");
            }
            finally
            {
                _driver.Close();
            }
        }


        [Test]
        public void WaitForUserTest()
        {
            _driver.Navigate().GoToUrl("https://www.seleniumeasy.com/test/dynamic-data-loading-demo.html");
            _driver.FindElement(By.Id("save")).Click();


            Assert.IsTrue(wait(_driver, By.XPath("//div[@id=\"loading\"]/img[contains(@src, \"https://randomuser.me\")]")));
        }
    }
}