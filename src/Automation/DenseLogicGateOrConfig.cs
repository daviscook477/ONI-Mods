using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STRINGS;

namespace Automation
{
    public class DenseLogicGateOrConfig : DenseLogicGateBaseConfig
    {
        public const string Id = "DenseLogicGateOR";

        protected override LogicGateBase.Op GetLogicOp()
        {
            return LogicGateBase.Op.Or;
        }

        protected override CellOffset[] InputPortOffsets
        {
            get
            {
                return new CellOffset[2]
                {
                    CellOffset.none,
                    new CellOffset(0, 1)
                };
            }
        }

        protected override CellOffset[] OutputPortOffsets
        {
            get
            {
                return new CellOffset[1] { new CellOffset(1, 0) };
            }
        }

        protected override DenseLogicGate.DenseLogicGateDescriptions GetDescriptions()
        {
            return new DenseLogicGate.DenseLogicGateDescriptions()
            {
                outputOne = new DenseLogicGate.DenseLogicGateDescriptions.Description()
                {
                    name = BUILDINGS.PREFABS.LOGICGATEOR.OUTPUT_NAME,
                    active = BUILDINGS.PREFABS.LOGICGATEOR.OUTPUT_ACTIVE,
                    inactive = BUILDINGS.PREFABS.LOGICGATEOR.OUTPUT_INACTIVE
                }
            };
        }

        public override BuildingDef CreateBuildingDef()
        {
            return CreateBuildingDef(Id, "or_dense_kanim", 2, 2);
        }
    }
}
