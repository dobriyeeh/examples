using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActualData;
using DataModel;

namespace CMSController
{
    public enum FixedIssues
    {
        DuplicatedSpace,
        CommaAfterCities,
        MissedIds
    }

    public interface EntityInformationFixer
    {
        EntityInformation Fix(EntityInformation originalInformation);
    }

    public class EntityInformationFixerFactory
    {
        public static EntityInformationFixer Create(FixedIssues issues)
        {
            switch (issues)
            {
                case FixedIssues.DuplicatedSpace:
                    return new SpaceInNamesFixer();

                case FixedIssues.CommaAfterCities:
                    return new CommaAfterCitiesFixer();
            }

            throw new Exception("not implemented yet");
        }

        private class SpaceInNamesFixer : EntityInformationFixer
        {
            public EntityInformation Fix(EntityInformation original)
            {
                original.EntityName = WebPagesEntityInfoParser.DataDecode(original.EntityName);
                original.AgentForServiceOfProcess = WebPagesEntityInfoParser.DataDecode(original.AgentForServiceOfProcess);
                original.EntityCityStateZip = WebPagesEntityInfoParser.DataDecode(original.EntityCityStateZip);
                original.EntityAddress = WebPagesEntityInfoParser.DataDecode(original.EntityAddress);

                return original;
            }
        }

        private class CommaAfterCitiesFixer : EntityInformationFixer
        {
            public EntityInformation Fix(EntityInformation original)
            {
                original.EntityCityStateZip = WebPagesEntityInfoParser.AddressStateFix(original.EntityCityStateZip);
                return original;
            }
        }
    }
}
