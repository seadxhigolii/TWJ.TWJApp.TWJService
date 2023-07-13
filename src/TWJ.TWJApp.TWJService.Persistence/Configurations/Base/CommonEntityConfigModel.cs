using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Persistence.Configurations.Base
{
    public class CommonEntityConfigModel
    {
        public record BaseEntityConfigModel(bool IsKey, string Property, string Value, bool IsRequired = true);
        public static string Id => "Id";
        public static string CreatedAt => "CreatedAt";
        public static string UpdatedAt => "UpdatedAt";

        public static IList<BaseEntityConfigModel> GetBaseConfigModels()
        {
            return new List<BaseEntityConfigModel> {
                new BaseEntityConfigModel(true, Id, "ID"),
                new BaseEntityConfigModel(false, CreatedAt, "CreatedAt"),
                new BaseEntityConfigModel(false, UpdatedAt, "UpdatedAt"),
            };
        }
    }
}
