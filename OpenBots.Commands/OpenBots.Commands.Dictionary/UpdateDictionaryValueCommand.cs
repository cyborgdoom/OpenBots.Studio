﻿using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Properties;
using OpenBots.Core.Utilities.CommonUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Windows.Forms;
using Microsoft.Office.Interop.Outlook;
using MimeKit;
using OpenQA.Selenium;
using Exception = System.Exception;

namespace OpenBots.Commands.Dictionary
{
    [Serializable]
    [Category("Dictionary Commands")]
    [Description("This command updates the value in an existing Dictionary variable for a specified key.")]
    public class UpdateDictionaryValueCommand : ScriptCommand
	{
		[Required]
		[DisplayName("Dictionary")]
		[Description("Provide a Dictionary variable.")]
		[SampleUsage("{vDictionary}")]
		[Remarks("Any type of variable other than Dictionary will cause error.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_DictionaryName { get; set; }

		[Required]
		[DisplayName("Key")]
		[Description("Enter the Key where the value will be updated.")]
		[SampleUsage("Hello || {vKey}")]
		[Remarks("Providing a non existing key will produce an exception.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_Key { get; set; }

		[Required]
		[DisplayName("Value")]
		[Description("Enter the value to write to the Dictionary.")]
		[SampleUsage("Hello || {vValue}")]
		[Remarks("Value can only be a String, DataTable, MailItem or IWebElement.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_Value { get; set; }

		public UpdateDictionaryValueCommand()
		{
			CommandName = "UpdateDictionaryValueCommand";
			SelectionName = "Update Dictionary Value";
			CommandEnabled = true;
			CommandIcon = Resources.command_dictionary;
		}

		public override void RunCommand(object sender)
		{
			//get sending instance
			var engine = (IAutomationEngineInstance)sender;

			var vDictionaryVariable = v_DictionaryName.ConvertUserVariableToObject(engine);
			var vKey = v_Key.ConvertUserVariableToString(engine);

			if (vDictionaryVariable != null)
			{
				if (vDictionaryVariable is Dictionary<string, string>)
				{
					((Dictionary<string, string>)vDictionaryVariable)[vKey] = v_Value.ConvertUserVariableToString(engine);
				}
				else if (vDictionaryVariable is Dictionary<string,DataTable>)
				{
					DataTable dataTable;
					var dataTableVariable = v_Value.Trim().ConvertUserVariableToObject(engine);
					if (dataTableVariable != null && dataTableVariable is DataTable)
						dataTable = (DataTable)dataTableVariable;
					else
						throw new DataException("Invalid Value type, please provide valid Value type.");
					((Dictionary<string, DataTable>)vDictionaryVariable)[vKey] = dataTable;
				}
				else if (vDictionaryVariable is Dictionary<string, MailItem>)
				{
					MailItem mailItem;
					var mailItemVariable = v_Value.Trim().ConvertUserVariableToObject(engine);
					if (mailItemVariable != null && mailItemVariable is MailItem)
						mailItem = (MailItem)mailItemVariable;
					else
						throw new DataException("Invalid Value type, please provide valid Value type.");
					((Dictionary<string, MailItem>)vDictionaryVariable)[vKey] = mailItem;
				}
				else if (vDictionaryVariable is Dictionary<string, MimeMessage>)
				{
					MimeMessage mimeMessage;
					var mimeMessageVariable = v_Value.Trim().ConvertUserVariableToObject(engine);
					if (mimeMessageVariable != null && mimeMessageVariable is MimeMessage)
						mimeMessage = (MimeMessage)mimeMessageVariable;
					else
						throw new DataException("Invalid Value type, please provide valid Value type.");
					((Dictionary<string, MimeMessage>)vDictionaryVariable)[vKey] = mimeMessage;
				}
				else if (vDictionaryVariable is Dictionary<string, IWebElement>)
				{
					IWebElement webElement;
					var webElementVariable = v_Value.Trim().ConvertUserVariableToObject(engine);
					if (webElementVariable != null && webElementVariable is IWebElement)
						webElement = (IWebElement)webElementVariable;
					else
						throw new DataException("Invalid Value type, please provide valid Value type.");
					((Dictionary<string, IWebElement>)vDictionaryVariable)[vKey] = webElement;
				}
				else if (vDictionaryVariable is Dictionary<string, object>)
				{
					object objectItem;
					var objectItemVariable = v_Value.Trim().ConvertUserVariableToObject(engine);
					if (objectItemVariable != null && objectItemVariable is object)
						objectItem = (object)objectItemVariable;
					else
						throw new DataException("Invalid Value type, please provide valid Value type.");
					((Dictionary<string, object>)vDictionaryVariable)[vKey] = objectItem;
				}
				else
					throw new DataException("Invalid dictionary type, please provide valid dictionary type.");
			}
			else
				throw new NullReferenceException("Attempted to write data to a variable, but the variable was not found. Enclose variables within braces, ex. {vVariable}");
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_DictionaryName", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_Key", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_Value", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [Write Value '{v_Value}' to Dictionary '{v_DictionaryName}' at Key '{v_Key}']";
		}
	}
}
