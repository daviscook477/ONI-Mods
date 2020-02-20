using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STRINGS;

namespace Automation
{
    public class DenseLogicGateNotConfig : DenseLogicGateBaseConfig
    {
        public const string Id = "DenseLogicGateNOT";

        protected override LogicGateBase.Op GetLogicOp()
        {
            return LogicGateBase.Op.Not;
        }

        protected override CellOffset[] InputPortOffsets
        {
            get
            {
                return new CellOffset[1]
                {
                    CellOffset.none
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
                    name = BUILDINGS.PREFABS.LOGICGATENOT.OUTPUT_NAME,
                    active = BUILDINGS.PREFABS.LOGICGATENOT.OUTPUT_ACTIVE,
                    inactive = BUILDINGS.PREFABS.LOGICGATENOT.OUTPUT_INACTIVE
                }
            };
        }

        public override BuildingDef CreateBuildingDef()
        {
            return CreateBuildingDef(Id, "not_dense_kanim", 2, 1);
        }
    }
}
