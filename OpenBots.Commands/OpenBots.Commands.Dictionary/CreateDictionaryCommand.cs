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
	[Description("This command creates a new Dictionary.")]
	public class CreateDictionaryCommand : ScriptCommand
	{
		[Required]
		[DisplayName("Dictionary Type")]
		[PropertyUISelectionOption("String")]
		[PropertyUISelectionOption("DataTable")]
		[PropertyUISelectionOption("MailItem (Outlook)")]
		[PropertyUISelectionOption("MimeMessage (IMAP/SMTP)")]
		[PropertyUISelectionOption("IWebElement")]
		[PropertyUISelectionOption("Object")]
		[Description("Specify the data type of values in dictionary.")]
		[SampleUsage("")]
		[Remarks("")]
		public string v_DictionaryType { get; set; }

		[Required]
		[DisplayName("Keys and Values")]
		[Description("Enter the Keys and Values required for the new dictionary.")]
		[SampleUsage("[FirstName | John] || [{vKey} | {vValue}]")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public DataTable v_ColumnNameDataTable { get; set; }

		[Required]
		[Editable(false)]
		[DisplayName("Output Dictionary Variable")]
		[Description("Create a new variable or select a variable from the list.")]
		[SampleUsage("{vUserVariable}")]
		[Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
		public string v_OutputUserVariableName { get; set; }

		public CreateDictionaryCommand()
		{
			CommandName = "CreateDictionaryCommand";
			SelectionName = "Create Dictionary";
			CommandEnabled = true;
			CommandIcon = Resources.command_dictionary;

			//initialize Datatable
			v_ColumnNameDataTable = new DataTable
			{
				TableName = "ColumnNamesDataTable" + DateTime.Now.ToString("MMddyy.hhmmss")
			};

			v_ColumnNameDataTable.Columns.Add("Keys");
			v_ColumnNameDataTable.Columns.Add("Values");
			v_DictionaryType = "String";
		}

		public override void RunCommand(object sender)
		{
			var engine = (IAutomationEngineInstance)sender;

			dynamic outputDictionary = null;

			switch (v_DictionaryType)
			{
				case "String":
					outputDictionary = new Dictionary<string, string>(); 
                        foreach (DataRow rwColumnName in v_ColumnNameDataTable.Rows)
                        {
                            outputDictionary.Add(
                                rwColumnName.Field<string>("Keys").ConvertUserVariableToString(engine),
                                rwColumnName.Field<string>("Values").ConvertUserVariableToString(engine));
                        }
					break;
				case "DataTable":
					outputDictionary = new Dictionary<string, DataTable>();
						foreach (DataRow rwColumnName in v_ColumnNameDataTable.Rows)
						{
							DataTable dataTable;
							var dataTableVariable = rwColumnName.Field<string>("Values").ConvertUserVariableToObject(engine);
							if (dataTableVariable != null && dataTableVariable is DataTable)
								dataTable = (DataTable)dataTableVariable;
							else
								throw new DataException("Invalid dictionary value type, please provide valid dictionary value type.");
							outputDictionary.Add(
								rwColumnName.Field<string>("Keys").ConvertUserVariableToString(engine), dataTable);
						}
					break;
				case "MailItem (Outlook)":
					outputDictionary = new Dictionary<string, MailItem>();
					foreach (DataRow rwColumnName in v_ColumnNameDataTable.Rows)
						{
							MailItem mailItem;
							var mailItemVariable = rwColumnName.Field<string>("Values").ConvertUserVariableToObject(engine);
							if (mailItemVariable != null && mailItemVariable is MailItem)
								mailItem = (MailItem)mailItemVariable;
							else
								throw new DataException("Invalid dictionary value type, please provide valid dictionary value type.");
							outputDictionary.Add(
								rwColumnName.Field<string>("Keys").ConvertUserVariableToString(engine), mailItem);
						}
					break;
				case "MimeMessage (IMAP/SMTP)":
					outputDictionary = new Dictionary<string, MimeMessage>();
					foreach (DataRow rwColumnName in v_ColumnNameDataTable.Rows)
						{
							MimeMessage mimeMessage;
							var mimeMessageVariable = rwColumnName.Field<string>("Values").ConvertUserVariableToObject(engine);
							if (mimeMessageVariable != null && mimeMessageVariable is MimeMessage)
								mimeMessage = (MimeMessage)mimeMessageVariable;
							else
							throw new DataException("Invalid dictionary value type, please provide valid dictionary value type.");
							outputDictionary.Add(
								rwColumnName.Field<string>("Keys").ConvertUserVariableToString(engine), mimeMessage);
						}
					break;
				case "IWebElement":
					outputDictionary = new Dictionary<string, IWebElement>();
					foreach (DataRow rwColumnName in v_ColumnNameDataTable.Rows)
						{
							IWebElement webElement;
							var webElementVariable = rwColumnName.Field<string>("Values").ConvertUserVariableToObject(engine);
							if (webElementVariable != null && webElementVariable is IWebElement)
								webElement = (IWebElement)webElementVariable;
							else
								throw new DataException("Invalid dictionary value type, please provide valid dictionary value type.");
							outputDictionary.Add(
								rwColumnName.Field<string>("Keys").ConvertUserVariableToString(engine), webElement);
						}
					break;
				case "Object":
					outputDictionary = new Dictionary<string, object>();
					foreach (DataRow rwColumnName in v_ColumnNameDataTable.Rows)
					{
						object objectItem;
						var objectItemVariable = rwColumnName.Field<string>("Values").ConvertUserVariableToObject(engine);
						if (objectItemVariable != null && objectItemVariable is object)
							objectItem = (object)objectItemVariable;
						else
							throw new DataException("Invalid dictionary value type, please provide valid dictionary value type.");
						outputDictionary.Add(
							rwColumnName.Field<string>("Keys").ConvertUserVariableToString(engine), objectItem);
					}
					break;
			}

			((object)outputDictionary).StoreInUserVariable(engine, v_OutputUserVariableName);
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_DictionaryType", this, editor));
			RenderedControls.AddRange(commandControls.CreateDataGridViewGroupFor("v_ColumnNameDataTable", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [With {v_ColumnNameDataTable.Rows.Count} Entries - Store Dictionary in '{v_OutputUserVariableName}']";
		}
	}
}