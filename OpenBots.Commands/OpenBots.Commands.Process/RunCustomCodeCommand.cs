using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Properties;
using OpenBots.Core.ScriptEngine;
using OpenBots.Core.Utilities.CommonUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace OpenBots.Commands.Process
{
	[Serializable]
	[Category("Programs/Process Commands")]
	[Description("This command runs custom C# code. The code in this command is compiled and run at runtime when this command is invoked.")]

	public class RunCustomCodeCommand : ScriptCommand
	{

		[Required]
		[DisplayName("C# Code")]
		[Description("Enter the code to be executed or use the builder to create your custom C# code. " +
							"The builder contains a Hello World template that you can use to build from.")]
		[SampleUsage("{vString}.Remove() || {vCode}")]
		[Remarks("This command only supports the standard framework classes.")]
		[Editor("ShowCodeBuilder", typeof(UIAdditionalHelperType))]
		public string v_Code { get; set; }

		[DisplayName("Arguments (Optional)")]
		[Description("Enter arguments that the custom code will receive during execution, split them using commas.")]
		[SampleUsage("hello || {vArg} || hello,world || {vArg1},{vArg2}")]
		[Remarks("This input is optional.")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_Args { get; set; }

		[Required]
		[Editable(false)]
		[DisplayName("Output Data Variable")]
		[Description("Create a new variable or select a variable from the list.")]
		[SampleUsage("{vUserVariable}")]
		[Remarks("Variables not pre-defined in the Variable Manager will be automatically generated at runtime.")]
		public string v_OutputUserVariableName { get; set; }

		public RunCustomCodeCommand()
		{
			CommandName = "RunCustomCodeCommand";
			SelectionName = "Run Custom Code";
			CommandEnabled = true;
			CommandIcon = Resources.command_script;

		}

		public override void RunCommand(object sender)
		{
			var engine = (IAutomationEngineInstance)sender;
			var customCode = v_Code.ConvertUserVariableToString(engine);

			var result = ScriptEngineCompiler.Compile(customCode);

			if (v_OutputUserVariableName.Length != 0)
			{
				((object)result).StoreInUserVariable(engine, v_OutputUserVariableName);
			}

		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_Code", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_Args", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultOutputGroupFor("v_OutputUserVariableName", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [Store Data in '{v_OutputUserVariableName}']";
		}

	}
}

