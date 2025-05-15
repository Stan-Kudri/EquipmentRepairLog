using Ardalis.SmartEnum;

namespace EquipmentRepairLog.Core.Data.Users
{
    public class UserRole : SmartEnum<UserRole>
    {
        /// <summary>
        /// Администратор
        /// </summary>
        public static UserRole Admin = new(Role.Admin, 0);

        /// <summary>
        /// Просмотрщик - пользователь может просматривать записи 
        /// </summary>
        public static UserRole Viewer = new(Role.UserViewer, 1);

        /// <summary>
        /// Пользователь - просматривает и изменяет/добавляет/удаляет записи
        /// </summary>
        public static UserRole Editor = new(Role.UserEditor, 2);

        public UserRole(string name, int value)
            : base(name, value)
        {
        }

        public override string ToString() => Name;
    }
}
