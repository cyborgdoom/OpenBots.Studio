﻿using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System;
using System.IO;
using Xunit;

namespace OpenBots.Commands.Folder.Test
{
    public class CreateFolderCommandTests
    {
        private AutomationEngineInstance _engine;
        private CreateFolderCommand _createFolder;

        [Fact]
        public void CreatesFolder()
        {
            _engine = new AutomationEngineInstance(null, null);
            _createFolder = new CreateFolderCommand();

            string folderName = "newFolder";
            folderName.StoreInUserVariable(_engine, "{folderName}");

            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            string inputPath = Path.Combine(projectDirectory, @"Resources");
            inputPath.StoreInUserVariable(_engine, "{inputPath}");

            _createFolder.v_NewFolderName = "{folderName}";
            _createFolder.v_DestinationDirectory = "{inputPath}";
            _createFolder.v_DeleteExisting = "Yes";

            _createFolder.RunCommand(_engine);

            string finalPath = Path.Combine(inputPath, folderName);

            Assert.True(Directory.Exists(finalPath));

            Directory.Delete(finalPath);
        }

        [Fact]
        public void HandlesInvalidFilepathInput()
        {
            _engine = new AutomationEngineInstance(null, null);
            _createFolder = new CreateFolderCommand();

            string folderName = "newFolder";
            folderName.StoreInUserVariable(_engine, "{folderName}");

            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            string inputPath = "notADirectoryPath";
            inputPath.StoreInUserVariable(_engine, "{inputPath}");

            _createFolder.v_NewFolderName = "{folderName}";
            _createFolder.v_DestinationDirectory = "{inputPath}";
            _createFolder.v_DeleteExisting = "Yes";

            Assert.Throws<DirectoryNotFoundException>(() => _createFolder.RunCommand(_engine));
        }
    }
}
