﻿using EquipmentRepairLog.Core.Data.DocumentModel;

namespace EquipmentRepairLog.Core.Data.StandardModel
{
    public class Perfomer : Entity
    {
        /// <summary>
        /// Название организации/цеха исполнителя работ
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Аббревиатура исполнителя работ
        /// </summary>
        public string Abbreviation { get; set; } = string.Empty;

        /// <summary>
        /// Список документов по исполнителю работ
        /// </summary>
        public List<Document>? Documents { get; set; } = new List<Document>();
    }
}
