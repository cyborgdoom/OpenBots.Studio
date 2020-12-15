﻿using MailKit;
using MailKit.Net.Imap;
using MimeKit;
using OpenBots.Core.Attributes.PropertyAttributes;
using OpenBots.Core.Command;
using OpenBots.Core.Enums;
using OpenBots.Core.Infrastructure;
using OpenBots.Core.Utilities.CommonUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Windows.Forms;

namespace OpenBots.Commands.Email
{
	[Serializable]
	[Category("Email Commands")]
	[Description("This command deletes a selected email using IMAP protocol.")]

	public class DeleteIMAPEmailCommand : ScriptCommand
	{

		[Required]
		[DisplayName("MimeMessage")]
		[Description("Enter the MimeMessage to delete.")]
		[SampleUsage("{vMimeMessage}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_IMAPMimeMessage { get; set; }

		[Required]
		[DisplayName("Host")]
		[Description("Define the host/service name that the script should use.")]
		[SampleUsage("imap.gmail.com || {vHost}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_IMAPHost { get; set; }

		[Required]
		[DisplayName("Port")]
		[Description("Define the port number that should be used when contacting the IMAP service.")]
		[SampleUsage("993 || {vPort}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_IMAPPort { get; set; }

		[Required]
		[DisplayName("Username")]
		[Description("Define the username to use when contacting the IMAP service.")]
		[SampleUsage("myRobot || {vUsername}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_IMAPUserName { get; set; }

		[Required]
		[DisplayName("Password")]
		[Description("Define the password to use when contacting the IMAP service.")]
		[SampleUsage("password || {vPassword}")]
		[Remarks("")]
		[Editor("ShowVariableHelper", typeof(UIAdditionalHelperType))]
		public string v_IMAPPassword { get; set; }

		[Required]
		[DisplayName("Delete Read Emails Only")]
		[PropertyUISelectionOption("Yes")]
		[PropertyUISelectionOption("No")]
		[Description("Specify whether to delete read email messages only.")]
		[SampleUsage("")]
		[Remarks("")]
		public string v_IMAPDeleteReadOnly { get; set; }

		public DeleteIMAPEmailCommand()
		{
			CommandName = "DeleteIMAPEmailCommand";
			SelectionName = "Delete IMAP Email";
			CommandEnabled = true;
			
			v_IMAPDeleteReadOnly = "Yes";
		}

		public override void RunCommand(object sender)
		{
			var engine = (IAutomationEngineInstance)sender;
			MimeMessage vMimeMessage = (MimeMessage)v_IMAPMimeMessage.ConvertUserVariableToObject(engine);
			string vIMAPHost = v_IMAPHost.ConvertUserVariableToString(engine);
			string vIMAPPort = v_IMAPPort.ConvertUserVariableToString(engine);
			string vIMAPUserName = v_IMAPUserName.ConvertUserVariableToString(engine);
			string vIMAPPassword = v_IMAPPassword.ConvertUserVariableToString(engine);

			using (var client = new ImapClient())
			{
				client.ServerCertificateValidationCallback = (sndr, certificate, chain, sslPolicyErrors) => true;
				client.SslProtocols = SslProtocols.None;

				using (var cancel = new CancellationTokenSource())
				{
					try
					{
						client.Connect(v_IMAPHost, int.Parse(v_IMAPPort), true, cancel.Token); //SSL
					}
					catch (Exception)
					{
						client.Connect(v_IMAPHost, int.Parse(v_IMAPPort)); //TLS
					}

					client.AuthenticationMechanisms.Remove("XOAUTH2");
					client.Authenticate(vIMAPUserName, vIMAPPassword, cancel.Token);

					var splitId = vMimeMessage.MessageId.Split('#').ToList();
					UniqueId messageId = UniqueId.Parse(splitId.Last());
					splitId.RemoveAt(splitId.Count - 1);
					string messageFolder = string.Join("", splitId);

					IMailFolder toplevel = client.GetFolder(client.PersonalNamespaces[0]);
					IMailFolder foundSourceFolder = GetIMAPEmailsCommand.FindFolder(toplevel, messageFolder);

					if (foundSourceFolder != null)
						foundSourceFolder.Open(FolderAccess.ReadWrite, cancel.Token);
					else
						throw new Exception("Source Folder not found");

					var messageSummary = foundSourceFolder.Fetch(new[] { messageId }, MessageSummaryItems.Flags);

					if (v_IMAPDeleteReadOnly == "Yes")
					{
						if (messageSummary[0].Flags.Value.HasFlag(MessageFlags.Seen))
							foundSourceFolder.AddFlags(messageId, MessageFlags.Deleted, true, cancel.Token);
					}
					else
						foundSourceFolder.AddFlags(messageId, MessageFlags.Deleted, true, cancel.Token);

					client.Disconnect(true, cancel.Token);
					client.ServerCertificateValidationCallback = null;
				}
			}
		}

		public override List<Control> Render(IfrmCommandEditor editor, ICommandControls commandControls)
		{
			base.Render(editor, commandControls);

			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_IMAPMimeMessage", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_IMAPHost", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_IMAPPort", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultInputGroupFor("v_IMAPUserName", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultPasswordInputGroupFor("v_IMAPPassword", this, editor));
			RenderedControls.AddRange(commandControls.CreateDefaultDropdownGroupFor("v_IMAPDeleteReadOnly", this, editor));

			return RenderedControls;
		}

		public override string GetDisplayValue()
		{
			return base.GetDisplayValue() + $" [MimeMessage '{v_IMAPMimeMessage}']";
		}
	}
}