
using Business.BusinessAspects;
using Business.Handlers.Parents.FilterParent;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.ParentDto;

namespace Business.Handlers.Parents.Queries
{
    public class GetParentQuery : IRequest<IDataResult<ParentGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetParentQueryHandler : IRequestHandler<GetParentQuery, IDataResult<ParentGetByIdDto>>
        {
            private readonly IParentRepository _parentRepository;
            private readonly IMediator _mediator;

            public GetParentQueryHandler(IParentRepository parentRepository, IMediator mediator)
            {
                _parentRepository = parentRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<ParentGetByIdDto>> Handle(GetParentQuery request, CancellationToken cancellationToken)
            {
                var parent = await _parentRepository.GetAsync(ParentFiltersHelper.GetParentQueryFilter(request));
                if (parent == null)
                    return new ErrorDataResult<ParentGetByIdDto>("Kayıt bulunamadı.");

                var dto = new ParentGetByIdDto
                {
                    Id = parent.Id,
                    PersonId = parent.PersonId
                };
                return new SuccessDataResult<ParentGetByIdDto>(dto);
            }
        }
    }
}
