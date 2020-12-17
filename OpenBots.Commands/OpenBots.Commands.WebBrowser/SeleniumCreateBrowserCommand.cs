﻿using OpenBots.Core.App;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Windows.Forms;

namespace OpenBots.Commands.WebBrowser
{
	[Serializable]
	[Category("Web Browser Commands")]
	[Description("This command creates a new Selenium web browser session which enables automation for websites.")]

	public class SeleniumCreateBrowserCommand : ScriptCommand, ISeleniumCreateBrowserCommand
	{

		[Required]
		[DisplayName("Browser Instance Name")]
		[Description("Enter a unique name that will represent the application instance.")]
		[SampleUsage("MyBrowserInstance")]
		[Remarks("This unique name allows you to refer to the instance by name in future commands, " +
				 "ensuring that the commands you specify run against the correct application.")]
		public string v_InstanceName { get; set; }

		[Required]
		[DisplayName("Browser Engine Type")]
		[PropertyUISelectionOption("Chrome")]
		[PropertyUISelectionOption("Firefox")]
		[PropertyUISelectionOption("Internet Explorer")]
		[Description("Select the browser engine to execute the Selenium automation with.")]
		[SampleUsage("")]
		[Remarks("The recommended browser option for web automation is Chrome.\n" + 
				 "If the IE engine is selected, make sure IE's security settings " +
				 "have 'Enabled Protected Mode' set to true across all zones.")]
		public string v_EngineType { get; set; }

		[DisplayName("URL (Optional)")]
		[Description("Enter the URL that you want the selenium instance to navigate to.")]
		[SampleUsage("https://mycompany.com/orders || {vURL}")]
		[Remarks("This input is optional.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_URL { get; set; }

		[Required]
		[DisplayName("Instance Tracking")]
		[PropertyUISelectionOption("Forget Instance")]
		[PropertyUISelectionOption("Keep Instance Alive")]
		[Description("Select **Forget Instance** to forget the instance after execution finishes, " +
							"or **Keep Instance Alive** to allow subsequent tasks to call the instance by name.")]
		[SampleUsage("")]
		[Remarks("Calling the **Close Browser** command or ending the browser session will end the instance. " +
				 "This command only works during the lifetime of the application. " +
				 "If the application is closed, the references will be forgotten automatically.")]
		public string v_InstanceTracking { get; set; }

		[Required]
		[DisplayName("Window State")]
		[PropertyUISelectionOption("Normal")]
		[PropertyUISelectionOption("Maximize")]
		[Description("Select the window state that the browser should start up with.")]
		[SampleUsage("")]
		[Remarks("")]
		public string v_BrowserWindowOption { get; set; }

		[DisplayName("Selenium Command Line Options (Chrome - Optional)")]
		[Description("Select options to be passed to the Selenium command.")]
		[SampleUsage("user-data-dir=c:\\users\\public\\SeleniumOpenBotsProfile || {vOptions}")]
		[Remarks("This input is optional.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_SeleniumOptions { get; set; }

		public SeleniumCreateBrowserCommand()
		{
			CommandName = "SeleniumCreateBrowserCommand";
			SelectionName = "Create Browser";
			CommandEnabled = true;
			
			v_InstanceName = "DefaultBrowser";
			v_InstanceTracking = "Forget Instance";
			v_BrowserWindowOption = "Maximize";
			v_EngineType = "Chrome";
			v_URL = "https://";
		}

		public override void RunCommand(object sender)
		{
			var engine = (AutomationEngineInstance)sender;
			var convertedOptions = v_SeleniumOptions.ConvertUserVariableToString(engine);
			var vURL = v_URL.ConvertUserVariableToString(engine);

			IWebDriver webDriver;

			string driverDirectory;
			if (engine.ScriptEngineUI != null && engine.ScriptEngineUI.IsScheduledTask)
				driverDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "OpenBots Inc", "OpenBots Studio");
			else
				driverDirectory = AppDomain.CurrentDomain.BaseDirectory;

			switch (v_EngineType)
			{
				case "Chrome":
					ChromeOptions chromeOptions = new ChromeOptions();
					chromeOptions.AddUserProfilePreference("download.prompt_for_download", true);

					if (!string.IsNullOrEmpty(convertedOptions.Trim()))
						chromeOptions.AddArguments(convertedOptions);

					webDriver = new ChromeDriver(driverDirectory, chromeOptions);
					break;

				case "Firefox":
					string firefoxExecutablePath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
					if (!File.Exists(firefoxExecutablePath))
						throw new FileNotFoundException($"Could not locate '{firefoxExecutablePath}'");

					FirefoxOptions firefoxOptions = new FirefoxOptions();
					firefoxOptions.BrowserExecutableLocation = firefoxExecutablePath;

					webDriver = new FirefoxDriver(driverDirectory, firefoxOptions);
					break;

				case "Internet Explorer":
					InternetExplorerOptions ieOptions = new InternetExplorerOptions();
					ieOptions.IgnoreZoomLevel = true;

					webDriver = new InternetExplorerDriver(driverDirectory, ieOptions);
					break;

				default:
					throw new Exception($"The selected engine type '{v_EngineType}' is not valid.");
			}

			//add app instance
			webDriver.AddAppInstance(engine, v_InstanceName);

			//handle app instance tracking
			if (v_InstanceTracking == "Keep Instance Alive")
				GlobalAppInstances.AddInstance(v_InstanceName, webDriver);

			switch (v_BrowserWindowOption)
			{
				case "Maximize":
					webDriver.Manage().Window.Maximize();
					break;
				case "Normal":
				case "":
				default:
					break;
			}

			if (!string.IsNullOrEmpty(vURL.Trim()) && vURL.Trim() != "https://")
				{
				try
				{
					webDriver.Navigate().GoToUrl(vURL);
				}
				catch (Exception ex)
				{
					if (!vURL.StartsWith("https://"))
						webDriver.Navigate().GoToUrl("https://" + vURL);
					else
						throw ex;
				}
			}
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_InstanceName", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_EngineType", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_URL", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_InstanceTracking", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_BrowserWindowOption", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_SeleniumOptions", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return $"Create {v_EngineType} Browser [Navigate To URL '{v_URL}' - New Instance Name '{v_InstanceName}']";
		}
	}
}
