﻿// Generated by Selenium IDE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using NUnit.Framework;
[TestFixture]
public class ApplyPermissionTest
{
    private IWebDriver driver;
    public IDictionary<string, object> vars { get; private set; }
    private IJavaScriptExecutor js;
    [SetUp]
    public void SetUp()
    {
        driver = new ChromeDriver();
        js = (IJavaScriptExecutor)driver;
        vars = new Dictionary<string, object>();
    }
    [TearDown]
    protected void TearDown()
    {
        driver.Quit();
    }
    [Test]
    public void applyPermission()
    {
        driver.Navigate().GoToUrl("http://localhost:3000/employeeprofile/6");
        driver.Manage().Window.Size = new System.Drawing.Size(1382, 744);
        driver.FindElement(By.CssSelector("a:nth-child(2) .item-content")).Click();
        {
            var element = driver.FindElement(By.CssSelector(".active .item-content"));
            Actions builder = new Actions(driver);
            builder.MoveToElement(element).Perform();
        }
        {
            var element = driver.FindElement(By.tagName("body"));
            Actions builder = new Actions(driver);
            builder.MoveToElement(element, 0, 0).Perform();
        }
        driver.FindElement(By.Id("cl-radio")).Click();
        driver.FindElement(By.Name("fromDate")).Click();
        driver.FindElement(By.Name("fromDate")).SendKeys("2024-04-01");
        driver.FindElement(By.CssSelector(".mt-4 > .col:nth-child(2) .form-control")).Click();
        driver.FindElement(By.CssSelector(".mt-4 > .col:nth-child(2) .form-control")).SendKeys("12:05");
        driver.FindElement(By.CssSelector(".row:nth-child(4) > .col:nth-child(2) > .mb-3")).Click();
        driver.FindElement(By.Name("reason")).Click();
        driver.FindElement(By.Name("reason")).SendKeys("Personel");
        driver.FindElement(By.CssSelector(".btn")).Click();
        driver.Close();
    }
}
