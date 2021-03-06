﻿using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System.Data;
using Xunit;

namespace OpenBots.Commands.Asset.Test
{
    public class CalculateNumberAssetCommandTests
    {
        private AutomationEngineInstance _engine;
        private CalculateNumberAssetCommand _calculateAsset;
        private GetAssetCommand _getAsset;
        private UpdateAssetCommand _updateAsset;

        [Fact]
        public void IncrementsNumberAsset()
        {
            _engine = new AutomationEngineInstance(null);
            _calculateAsset = new CalculateNumberAssetCommand();
            _getAsset = new GetAssetCommand();

            string assetName = "testIncrementNumberAsset";
            string newAsset = "50";
            assetName.StoreInUserVariable(_engine, "{assetName}");
            newAsset.StoreInUserVariable(_engine, "{newAsset}");

            _calculateAsset.v_AssetName = "{assetName}";
            _calculateAsset.v_AssetActionType = "Increment";
            _calculateAsset.v_AssetActionValue = "";

            _calculateAsset.RunCommand(_engine);

            _getAsset.v_AssetName = "{assetName}";
            _getAsset.v_AssetType = "Number";
            _getAsset.v_OutputUserVariableName = "{output}";

            _getAsset.RunCommand(_engine);

            string outputAsset = "{output}".ConvertUserVariableToString(_engine);
            Assert.Equal(newAsset, outputAsset);

            resetAsset(assetName, "49", "Number");
        }

        [Fact]
        public void DecrementsNumberAsset()
        {
            _engine = new AutomationEngineInstance(null);
            _calculateAsset = new CalculateNumberAssetCommand();
            _getAsset = new GetAssetCommand();

            string assetName = "testIncrementNumberAsset";
            string newAsset = "48";
            assetName.StoreInUserVariable(_engine, "{assetName}");
            newAsset.StoreInUserVariable(_engine, "{newAsset}");

            _calculateAsset.v_AssetName = "{assetName}";
            _calculateAsset.v_AssetActionType = "Decrement";
            _calculateAsset.v_AssetActionValue = "";

            _calculateAsset.RunCommand(_engine);

            _getAsset.v_AssetName = "{assetName}";
            _getAsset.v_AssetType = "Number";
            _getAsset.v_OutputUserVariableName = "{output}";

            _getAsset.RunCommand(_engine);

            string outputAsset = "{output}".ConvertUserVariableToString(_engine);
            Assert.Equal(newAsset, outputAsset);

            resetAsset(assetName, "49", "Number");
        }

        [Fact]
        public void AddsNumberAsset()
        {
            _engine = new AutomationEngineInstance(null);
            _calculateAsset = new CalculateNumberAssetCommand();
            _getAsset = new GetAssetCommand();

            string assetName = "testIncrementNumberAsset";
            string newAsset = "54";
            assetName.StoreInUserVariable(_engine, "{assetName}");
            newAsset.StoreInUserVariable(_engine, "{newAsset}");

            _calculateAsset.v_AssetName = "{assetName}";
            _calculateAsset.v_AssetActionType = "Add";
            _calculateAsset.v_AssetActionValue = "5";

            _calculateAsset.RunCommand(_engine);

            _getAsset.v_AssetName = "{assetName}";
            _getAsset.v_AssetType = "Number";
            _getAsset.v_OutputUserVariableName = "{output}";

            _getAsset.RunCommand(_engine);

            string outputAsset = "{output}".ConvertUserVariableToString(_engine);
            Assert.Equal(newAsset, outputAsset);

            resetAsset(assetName, "49", "Number");
        }

        [Fact]
        public void SubtractsNumberAsset()
        {
            _engine = new AutomationEngineInstance(null);
            _calculateAsset = new CalculateNumberAssetCommand();
            _getAsset = new GetAssetCommand();

            string assetName = "testIncrementNumberAsset";
            string newAsset = "43";
            assetName.StoreInUserVariable(_engine, "{assetName}");
            newAsset.StoreInUserVariable(_engine, "{newAsset}");

            _calculateAsset.v_AssetName = "{assetName}";
            _calculateAsset.v_AssetActionType = "Subtract";
            _calculateAsset.v_AssetActionValue = "6";

            _calculateAsset.RunCommand(_engine);

            _getAsset.v_AssetName = "{assetName}";
            _getAsset.v_AssetType = "Number";
            _getAsset.v_OutputUserVariableName = "{output}";

            _getAsset.RunCommand(_engine);

            string outputAsset = "{output}".ConvertUserVariableToString(_engine);
            Assert.Equal(newAsset, outputAsset);

            resetAsset(assetName, "49", "Number");
        }

        [Fact]
        public void HandlesNonexistentAsset()
        {
            _engine = new AutomationEngineInstance(null);
            _calculateAsset = new CalculateNumberAssetCommand();
            _getAsset = new GetAssetCommand();

            string assetName = "noAsset";
            string newAsset = "50";
            assetName.StoreInUserVariable(_engine, "{assetName}");
            newAsset.StoreInUserVariable(_engine, "{assetName}");

            _calculateAsset.v_AssetName = "{assetName}";
            _calculateAsset.v_AssetActionType = "Increment";
            _calculateAsset.v_AssetActionValue = "";

            Assert.Throws<DataException>(() => _calculateAsset.RunCommand(_engine));
        }

        private void resetAsset(string assetName, string assetValue, string type)
        {
            _engine = new AutomationEngineInstance(null);
            _updateAsset = new UpdateAssetCommand();

            _updateAsset.v_AssetName = assetName;
            _updateAsset.v_AssetType = type;
            _updateAsset.v_AssetValue = assetValue;

            _updateAsset.RunCommand(_engine);
        }
    }
}
