using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Collections.Generic;

namespace Task50
{
    public class Tests
    {
        IWebDriver _driver;
        private static string _mailUserNameXPath = "//div[@class=\"mail-User-Name\"]";
        private static string _mailUrl = "https://mail.tut.by";

        private static string _loginFieldId = "Username";
        private static string _passwrdFieldId = "Password";
        private static string _loginButtonXpath = "//input[contains(@class, \"loginButton\")]";
        private static string _confirmButtonXPath = "//button[@onclick=\"myConfirmFunction()\"]";
        private static string _multiselectElementsXPath = "//select[@id = \"multi-select\"]";
        private static string _confirmFunctionButtonXpath = "//button[@onclick=\"myConfirmFunction()\"]";
        private static string _promtFunctionButtonXpath = "//button[@onclick=\"myPromptFunction()\"]";
        private static string _userPicXPath = "//div[@id=\"loading\"]/img[contains(@src, \"https://randomuser.me\")]";

        private static string _multiSelectTestUrl = "https://www.seleniumeasy.com/test/basic-select-dropdown-demo.html";
        private static string _confirmOkAlertTest = "https://www.seleniumeasy.com/test/javascript-alert-box-demo.html";
        private static string _confirmCancelAlertsTestUrl = "https://www.seleniumeasy.com/test/javascript-alert-box-demo.html";
        private static string _alertBoxTestUrl = "https://www.seleniumeasy.com/test/javascript-alert-box-demo.html";
        private static string _waitForUserTestUrl = "https://www.seleniumeasy.com/test/dynamic-data-loading-demo.html";

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
        void MakeScreenShot(IWebDriver driver)
        {
            Screenshot img = ((ITakesScreenshot)driver).GetScreenshot();
            //string path = Environment.CurrentDirectory;
            img.SaveAsFile(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, ScreenshotImageFormat.Png);
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
            
            _driver.Navigate().GoToUrl(_mailUrl);

            MakeScreenShot(_driver); //making screenshot, saving it to the Desktop

            IWebElement loginField = _driver.FindElement(By.Id(_loginFieldId));
            IWebElement passwrdField = _driver.FindElement(By.Id(_passwrdFieldId));
            IWebElement loginButton = _driver.FindElement(By.XPath(_loginButtonXpath));

            loginField.SendKeys(loginstr);
            passwrdField.SendKeys(passwordstr);
            loginButton.Click();

         //   Thread.Sleep(10000); //neither explicit, nor implicit waiter. Implicit waiters work for all the elements. And the explicit waiters works for a particular element with the particular condition. Thread.Sleep() just stop the thread with executing code for a particular time.



        
            Assert.IsTrue(wait(_driver, By.XPath(_mailUserNameXPath)), "Login failed");
           
        }

        [Test]
        public void MultiSelectTest()
        {
            _driver.Navigate().GoToUrl(_multiSelectTestUrl);

            SelectElement selectedSates = new SelectElement(_driver.FindElement(By.XPath(_multiselectElementsXPath))); //add check for multiselect enabled

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

            Assert.IsTrue(result, "No items selected");
        }

        [Test]
        public void ConfirmOkAlertTest()
        {
            _driver.Navigate().GoToUrl(_confirmOkAlertTest);
            _driver.FindElement(By.XPath(_confirmButtonXPath)).Click();

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
            
        }


        [Test]
        public void ConfirmCancelAlertTest()
        {
            _driver.Navigate().GoToUrl(_confirmCancelAlertsTestUrl);
            _driver.FindElement(By.XPath(_confirmFunctionButtonXpath)).Click();

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

        }

        [TestCase("Test Name", "Test Name")]
        [Test]
        public void AlertBoxTest(string inValue, string outValue)
        {
            _driver.Navigate().GoToUrl(_alertBoxTestUrl);
            _driver.FindElement(By.XPath(_promtFunctionButtonXpath)).Click();

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

        }


        [Test]
        public void WaitForUserTest()
        {
            _driver.Navigate().GoToUrl(_waitForUserTestUrl);
            _driver.FindElement(By.Id("save")).Click();


            Assert.IsTrue(wait(_driver, By.XPath(_userPicXPath)), "User not displayed");
        }
    }
}