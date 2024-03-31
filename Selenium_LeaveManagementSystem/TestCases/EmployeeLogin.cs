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
public class EmployeeLoginTest
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
    public void employeeLogin()
    {
        driver.Navigate().GoToUrl("http://localhost:3000/login");
        driver.Manage().Window.Size = new System.Drawing.Size(527, 708);
        driver.FindElement(By.Id("floatingInput")).Click();
        driver.FindElement(By.Id("floatingInput")).SendKeys("ashamol2002@gmail.com");
        driver.FindElement(By.Id("floatingPassword")).Click();
        driver.FindElement(By.Id("floatingPassword")).SendKeys("sena123");
        driver.FindElement(By.Id("btn")).Click();
        driver.FindElement(By.CssSelector(".card")).Click();
        driver.Close();
    }
}