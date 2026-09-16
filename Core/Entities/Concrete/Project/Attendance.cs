using System;
using Core.Entities;

namespace Core.Entities.Concrete.Project
{
    // Yoklama Kaydı: Öğrencinin derse katılım durumu
    public class Attendance : TenantEntity,IEntity
    {
        public int CourseId { get; set; } // Ders ID
        public int StudentId { get; set; } // Öğrenci ID
        public DateTime AttendanceDate { get; set; } // Yoklama tarihi
        public bool IsPresent { get; set; } // Katıldı mı? (true: Var, false: Yok)
        public string Reason { get; set; } // Devamsızlık / mazeret nedeni

        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}
