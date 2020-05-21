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

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
        }

        [TestCase("seleniumtests@tut.by", "123456789zxcvbn")]
        [TestCase("seleniumtests2@tut.by", "123456789zxcvbn")]

        [Test]
        public void LoginTest(string loginstr, string passwordstr)
        {
            _driver.Navigate().GoToUrl("https://mail.tut.by");
            IWebElement loginField = _driver.FindElement(By.Id("Username"));
            IWebElement passwrdField = _driver.FindElement(By.Id("Password"));
            IWebElement loginButton = _driver.FindElement(By.XPath("//input[contains(@class, \"loginButton\")]"));

            loginField.SendKeys(loginstr);
            passwrdField.SendKeys(passwordstr);
            loginButton.Click();

         //   Thread.Sleep(10000); //neither explicit, nor implicit waiter. Implicit waiters work for all the elements. And the explicit waiters works for a particular element with the particular condition. Thread.Sleep() just stop the thread with executing code for a particular time.

            WebDriverWait loginWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15)); //creating explicit waiter
            loginWait.PollingInterval = TimeSpan.FromMilliseconds(100); //changing polling frequency

            try
            {
                var loginText = loginWait.Until(_driver => _driver.FindElement(By.XPath("//div[@class=\"mail-User-Name\"]")));
                Assert.IsTrue((loginText.Displayed) && (loginText.Text == loginstr), $"User name is not displayed or user name is not equal to {loginstr}");
            }
            catch (NoSuchElementException)
            {
                Assert.Fail("Login failed");
            }
            catch (StaleElementReferenceException)
            {
                Assert.Fail("User name element is stale");
            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail("Login failed");
            }
            finally
            {
                _driver.Quit();
            }
            
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

            _driver.Close();
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

            WebDriverWait userWaiter = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            try
            {
                var userPicDispl = userWaiter.Until(condition =>
                {
                    try
                    {
                        var userPic = _driver.FindElement(By.XPath("//div[@id=\"loading\"]/img[contains(@src, \"https://randomuser.me\")]"));
                        return userPic.Enabled;
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

                Assert.IsTrue(userPicDispl);
            }
            catch(WebDriverTimeoutException)
            {
                Assert.Fail("User is not displayed");
            }
            finally
            {
                _driver.Close();
            }

        }
    }
}