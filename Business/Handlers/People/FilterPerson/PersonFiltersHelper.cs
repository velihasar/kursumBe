using Business.Handlers.People.Commands;
using Business.Handlers.People.Queries;
using Core.Entities.Concrete.Project;
using Core.Extensions;
using System;
using System.Linq.Expressions;

namespace Business.Handlers.People.FilterPerson
{
    public static class PersonFiltersHelper
    {
        public static Expression<Func<Person, bool>> GetPersonQueryFilter(GetPersonQuery request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.Id == request.Id;
        }

        public static Expression<Func<Person, bool>> GetPeopleQueryFilter(GetPeopleQuery request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true;
        }

        public static Expression<Func<Person, bool>> CreatePersonCommandFilter(CreatePersonCommand request)
        {
            var tenantId = UserInfoExtensions.GetTenantIdOrZero();
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.Email == request.Email &&
                (tenantId <= 0 || c.TenantId == tenantId);
        }

        public static Expression<Func<Person, bool>> UpdatePersonCommandFilter(UpdatePersonCommand request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.Id == request.Id;
        }
    }
}
