using Ardalis.SmartEnum;

namespace EquipmentRepairLog.Core.Data.User
{
    public class UserRole : SmartEnum<UserRole>
    {
        public static UserRole Admin = new UserRole(Role.Admin, 0);

        public static UserRole Viewer = new UserRole(Role.UserViewer, 1);

        public static UserRole Editor = new UserRole(Role.UserEditor, 2);

        public UserRole(string name, int value)
            : base(name, value)
        {
        }

        public override string ToString() => Name;
    }
}
