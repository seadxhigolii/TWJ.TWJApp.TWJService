using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using TWJ.TWJApp.TWJService.Api.Controllers.Base;
using TWJ.TWJApp.TWJService.Application.Services.Account.Commands.Login;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateFilesController : BaseController
    {
        #region Login
        [HttpPost("Generate/{entityName}")]
        [AllowAnonymous]
        public async void Generate(string entityName)
        {
            GenerateFilesForEntity(entityName);
        }
        #endregion

        static void GenerateFilesForEntity(string entityName)
        {
            var rootDirectory = @"C:\Sources\thewellnessjunction\src\TWJ.TWJApp.TWJService.Application";

            var baseDirectory = Path.Combine(rootDirectory, "Services");

            var commandActions = new[]
            {
                new { Name = "Add", Classes = new[] { "Command", "CommandHandler", "CommandValidator" } },
                new { Name = "Delete", Classes = new[] { "Command", "CommandHandler" } },
                new { Name = "Update", Classes = new[] { "Command", "CommandHandler", "CommandValidator" } }
            };

            foreach (var action in commandActions)
            {
                foreach (var className in action.Classes)
                {
                    var (generatedClassName, content) = GenerateClass(entityName, action.Name, className);
                    var directory = Path.Combine(baseDirectory, entityName, "Commands", action.Name);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    System.IO.File.WriteAllText(Path.Combine(directory, generatedClassName + ".cs"), content);
                }
            }

            var queryActions = new[]
            {
                new { Name = "GetAll", Classes = new[] { "Model", "Query", "QueryHandler" } },
                new { Name = "GetById", Classes = new[] { "Model", "Query", "QueryHandler" } },
                new { Name = "GetFiltered", Classes = new[] { "Model", "Query", "QueryHandler" } }
            };

            foreach (var action in queryActions)
            {
                foreach (var className in action.Classes)
                {
                    var (generatedClassName, content) = GenerateClass(entityName, action.Name, className);
                    var directory = Path.Combine(baseDirectory, entityName, "Queries", action.Name);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    System.IO.File.WriteAllText(Path.Combine(directory, generatedClassName + ".cs"), content);
                }
            }
        }

        static (string ClassName, string Content) GenerateClass(string entityName, string action, string kind)
        {
            string className = "";
            string content = "";

            if (kind == "Command")
            {
                if (action == "Add")
                {
                    className = $"{action}{entityName}{kind}";
                    content = $@"using MediatR;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Add
{{
    public class {className} : IRequest<Unit>
    {{
        public string Property {{ get; set; }}

        public Domain.Entities.{entityName} Add{entityName}()
        {{
            return new Domain.Entities.{entityName}
            {{
                Property = Property
            }};
        }}
    }}
}}";
                }
                else if (action == "Delete")
                {
                    className = $"{action}{entityName}{kind}";
                    content = $@"using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Delete
{{
    public class {className} : IRequest<Unit>
    {{
        public Guid Id {{ get; set; }}
    }}
}}";
                }
                else if (action == "Update")
                {
                    className = $"{action}{entityName}{kind}";
                    content = $@"using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Update
{{
    public class {className} : IRequest<Unit>
    {{
        public Guid Id {{ get; set; }}
        public string Property {{ get; set; }}

        public Domain.Entities.{entityName} Update(Domain.Entities.{entityName} {entityName.ToLower()})
        {{
            {entityName.ToLower()}.Property = Property;
            return {entityName.ToLower()};
        }}
    }}
}}";
                }
            }
            else if (kind == "CommandHandler")
            {
                className = $"{action}{entityName}CommandHandler";
                if (action == "Add")
                {
                    content = $@"using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Add
{{
    public class {className} : IRequestHandler<Add{entityName}Command, Unit>
    {{
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = """";

        public {className}(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {{
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }}

        public async Task<Unit> Handle(Add{entityName}Command request, CancellationToken cancellationToken)
        {{
            try
            {{
                await _context.{entityName}s.AddAsync(request.Add{entityName}(), cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }}
            catch (Exception ex)
            {{
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception(""An error occurred while processing the request."", ex);
            }}
        }}
    }}
}}";
                }
                else if(action == "Delete")
                {
                    className = $"{action}{entityName}{kind}";
                    content = $@"using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Delete
{{
    public class {className} : IRequestHandler<Delete{entityName}Command, Unit>
    {{
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = """";

        public {className}(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {{
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }}

        public async Task<Unit> Handle(Delete{entityName}Command request, CancellationToken cancellationToken)
        {{
            try
            {{
                var data = await _context.{entityName}s.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (data == null) throw new BadRequestException(ValidatorMessages.NotFound(""Record""));

                _context.{entityName}s.Remove(data);

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }}
            catch (Exception ex)
            {{
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception(""An error occurred while processing the request."", ex);
            }}
        }}
    }}
}}";
                }
                else if (action == "Update")
                {
                    className = $"{action}{entityName}{kind}";
                    content = $@"using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Update
{{
    public class {className} : IRequestHandler<Update{entityName}Command, Unit>
    {{
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = """";

        public {className}(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {{
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }}

        public async Task<Unit> Handle(Update{entityName}Command request, CancellationToken cancellationToken)
        {{
            try
            {{
                var data = await _context.{entityName}s.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                _context.{entityName}s.Update(request.Update(data));

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }}
            catch (Exception ex)
            {{
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception(""An error occurred while processing the request."", ex);
            }}
        }}
    }}
}}";
                }
            }
            else if (kind == "CommandValidator")
            {
                if (action == "Add")
                {
                    className = $"{action}{entityName}CommandValidator";
                    content = $@"using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Add
{{
    public class {className} : AbstractValidator<Add{entityName}Command>
    {{
        private readonly ITWJAppDbContext _context;

        public {className}(ITWJAppDbContext context)
        {{
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Validations();
        }}

        private void Validations()
        {{
            RuleFor(x => x.Property).NotEmpty().WithMessage(ValidatorMessages.NotEmpty(""Property"")).DependentRules(() =>
            {{
                RuleFor(x => x.Property).MustAsync(async (name, cancellation) =>
                {{
                    return !await _context.{entityName}s.AsNoTracking().AnyAsync(x => x.Property.ToLower() == name.ToLower(), cancellation);
                }}).WithMessage(x => ValidatorMessages.AlreadyExists($""{entityName} with name {{x.Property}}""));
            }});
        }}
    }}
}}";
                }
                else if (action == "Update")
                {
                    className = $"{action}{entityName}{kind}";
                    content = $@"using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Update
{{
    public class {className} : AbstractValidator<Update{entityName}Command>
    {{
        private readonly ITWJAppDbContext _context;

        public {className}(ITWJAppDbContext context)
        {{
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Validations();
        }}

        private void Validations()
        {{
            RuleFor(x => x.Property).NotEmpty().WithMessage(ValidatorMessages.NotEmpty(""Id"")).DependentRules(() =>
            {{
                RuleFor(x => x.Property).MustAsync(async (id, cancellation) =>
                {{
                    return await _context.{entityName}s.AsNoTracking().AnyAsync(x => x.Property == id, cancellation);
                }}).WithMessage(ValidatorMessages.NotFound(""{entityName}"")).DependentRules(() =>
                {{
                    RuleFor(x => x.Property).MustAsync(async (args, id, cancellation) =>
                    {{
                        return !await _context.{entityName}s.AsNoTracking().Where(x => x.Property != id).AnyAsync(x => x.Property == args.Property, cancellation);
                    }}).WithMessage(x => ValidatorMessages.AlreadyExists($""{entityName} with Property {{x.Property}}""));
                }});
            }});

            RuleFor(x => x.Property).NotEmpty().WithMessage(ValidatorMessages.NotEmpty(""Property""));
        }}
    }}
}}";
                }

            }
            else if (kind == "Model")
            {
                if (action == "GetAll")
                {
                    className = $"{action}{entityName}{kind}";
                    content = $@"using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetAll
{{
    public class {className} : IProfile
    {{
        public Guid Id {{ get; set; }}
        public string Property {{ get; set; }}

        public async Task MapData(IProfileMapper profileMapper)
        {{
            profileMapper.Build<Domain.Entities.{entityName}, {className}>(
                (src, options) =>
                {{
                    return new {className}
                    {{
                        Id = src.Id,
                        Property = src.Property,
                    }};
                }});
        }}
    }}
}}";
                }
                else if (action == "GetById")
                {
                    className = $"Get{entityName}ByIdModel";
                    content = $@"using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetAll;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetById
{{
    public class {className} : IProfile
    {{
        public Guid Id {{ get; set; }}
        public string Name {{ get; set; }}

        public async Task MapData(IProfileMapper profileMapper)
        {{
            profileMapper.Build<Domain.Entities.{entityName}, {className}>(
                (src, options) =>
                {{
                    return new {className}
                    {{
                        Id = src.Id,
                        Property = src.Property,
                    }};
                }});
        }}
    }}
}}";
                }
                else if (action == "GetFiltered")
                {
                    className = $"GetFiltered{entityName}Model";
                    content = $@"using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetFiltered
{{
    public class {className} : IProfile
    {{
        public Guid Id {{ get; set; }}
        public string Property {{ get; set; }}

        public async Task MapData(IProfileMapper profileMapper)
        {{
            profileMapper.Build<Domain.Entities.{entityName}, {className}>(
                (src, options) =>
                {{
                    return new {className}
                    {{
                        Id = src.Id,
                        Property = src.Property,
                    }};
                }});
        }}
    }}
}}";
                }
            }
            else if (kind == "Query")
            {
                if (action == "GetAll")
                {
                    className = $"{action}{entityName}{kind}";
                    content = $@"using MediatR;
using System.Collections.Generic;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetAll
{{
    public class {className} : IRequest<IList<GetAll{entityName}Model>>
    {{
    }}
}}";
                }
                else if (action == "GetById")
                {
                    className = $"Get{entityName}ByIdQuery";
                    content = $@"using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetById
{{
    public class {className} : IRequest<Get{entityName}ByIdModel>
    {{
        public Guid Id {{ get; set; }}
    }}
}}";
                }
                else if (action == "GetFiltered")
                {
                    className = $"GetFiltered{entityName}Query";
                    content = $@"using MediatR;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetFiltered
{{
    public class {className} : FilterRequest, IRequest<FilterResponse<GetFiltered{entityName}Model>>
    {{
    }}
}}";
                }

            }
            else if (kind == "QueryHandler")
            {
                if (action == "GetAll")
                {
                    className = $"{action}{entityName}{kind}";
                    content = $@"using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetAll
{{
    public class {className} : IRequestHandler<GetAll{entityName}Query, IList<GetAll{entityName}Model>>
    {{
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = """";

        public {className}(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {{
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }}

        public async Task<IList<GetAll{entityName}Model>> Handle(GetAll{entityName}Query request, CancellationToken cancellationToken)
        {{
            try
            {{
                var data = await _context.{entityName}s
                                    .AsNoTracking()
                                    .OrderByDescending(x => x.Property)
                                    .ToListAsync(cancellationToken);

                var mappedData = data.Select(t => new GetAll{entityName}Model
                {{
                    Id = t.Id,
                    Property = t.Property
                }}).ToList();
                return mappedData;
            }}
            catch (Exception ex)
            {{
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception(""An error occurred while processing the request."", ex);
            }}
        }}
    }}
}}";
                }
                else if (action == "GetById")
                {
                    className = $"Get{entityName}ByIdQueryHandler";
                    content = $@"using MapperSegregator.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetById
{{
    public class {className} : IRequestHandler<Get{entityName}ByIdQuery, Get{entityName}ByIdModel>
    {{
        private readonly ITWJAppDbContext _context;
        private readonly IMapperSegregator _mapper;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = """";
        
        public {className}(ITWJAppDbContext context, IMapperSegregator mapper, IGlobalHelperService globalHelper)
        {{
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }}

        public async Task<Get{entityName}ByIdModel> Handle(Get{entityName}ByIdQuery request, CancellationToken cancellationToken)
        {{
            try
            {{
                var data = await _context.{entityName}.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (data == null) throw new BadRequestException(ValidatorMessages.NotFound(""{entityName}""));

                return await _mapper.MapAsync<Domain.Entities.{entityName}, Get{entityName}ByIdModel>(data);
            }}
            catch (Exception ex)
            {{
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception(""An error occurred while processing the request."", ex);
            }}
        }}
    }}
}}";
                }
                else if (action == "GetFiltered")
                {
                    className = $"GetFiltered{entityName}QueryHandler";
                    content = $@"using Asp.Nappox.School.Common.Extensions;
using MapperSegregator.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models.Base;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using TWJ.TWJApp.TWJService.Application.Dto.Commands.Base;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetFiltered
{{
    public class {className} : IRequestHandler<GetFiltered{entityName}Query, FilterResponse<GetFiltered{entityName}Model>>
    {{
        private readonly ITWJAppDbContext _context;
        private readonly IGlobalHelperService _globalHelper;
        private readonly string currentClassName = """";
        
        public {className}(ITWJAppDbContext context, IGlobalHelperService globalHelper)
        {{
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _globalHelper = globalHelper ?? throw new ArgumentNullException(nameof(globalHelper));
            currentClassName = GetType().Name;
        }}

        public async Task<FilterResponse<GetFiltered{entityName}Model>> Handle(GetFiltered{entityName}Query request, CancellationToken cancellationToken)
        {{
            try
            {{
                IQueryable<Domain.Entities.{entityName}> query = _context.{entityName}s.AsQueryable();
                
                query = query.ApplySorting(request.SortBy, request.SortDirection, topRecords: request.TopRecords);
                
                if (!request.TopRecords.HasValue)
                {{
                    query = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);
                }}

                var totalItems = await _context.{entityName}s.CountAsync(cancellationToken);

                var mappedData = await query.Select(src => new GetFiltered{entityName}Model
                {{
                    Id = src.Id
                }}).ToListAsync(cancellationToken);

                return new FilterResponse<GetFiltered{entityName}Model>
                {{
                    Data = mappedData,
                    TotalPages = (int)Math.Ceiling((double)totalItems / request.PageSize),
                    TotalItems = totalItems
                }};        

            }}
            catch (Exception ex)
            {{
                await _globalHelper.Log(ex, currentClassName);
                throw new Exception(""An error occurred while processing the request."", ex);
            }}
        }}
    }}
}}";
                }
            }
            else
            {
                className = "Unknown";
                content = "";
            }

            return (className, content);
        }

    }
}
