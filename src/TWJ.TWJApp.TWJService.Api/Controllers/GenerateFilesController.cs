using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using TWJ.TWJApp.TWJService.Application.Services.Account.Commands.Login;

namespace TWJ.TWJApp.TWJService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateFilesController : ControllerBase
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

            // Queries structure
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
                    content = $@"
using MediatR;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Add
{{
    public class {className} : IRequest<Unit>
    {{
        public string Property {{ get; set; }}

        public TWJ.TWJApp.TWJService.Domain.Entities.{entityName} Add{entityName}()
        {{
            return new TWJ.TWJApp.TWJService.Domain.Entities.{entityName}
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
                    content = $@"
using MediatR;
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
                    content = $@"
using MediatR;
using System;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Update
{{
    public class {className} : IRequest<Unit>
    {{
        public Guid Id {{ get; set; }}
        public string Property {{ get; set; }}

        public TWJ.TWJApp.TWJService.Domain.Entities.{entityName} Update(TWJ.TWJApp.TWJService.Domain.Entities.{entityName} {entityName.ToLower()})
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
                    content = $@"
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Add
{{
    public class {className} : IRequestHandler<Add{entityName}Command, Unit>
    {{
        private readonly ITWJAppDbContext _context;

        public {className}(ITWJAppDbContext context)
        {{
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }}

        public async Task<Unit> Handle(Add{entityName}Command request, CancellationToken cancellationToken)
        {{
            await _context.{entityName}.AddAsync(request.Add{entityName}(), cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }}
    }}
}}";
                }
                else if(action == "Delete")
                {
                    className = $"{action}{entityName}{kind}";
                    content = $@"
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Delete
{{
    public class {className} : IRequestHandler<Delete{entityName}Command, Unit>
    {{
        private readonly ITWJAppDbContext _context;

        public {className}(ITWJAppDbContext context)
        {{
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }}

        public async Task<Unit> Handle(Delete{entityName}Command request, CancellationToken cancellationToken)
        {{
            var data = await _context.{entityName}.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (data == null) throw new BadRequestException(ValidatorMessages.NotFound(""Record""));

            _context.{entityName}.Remove(data);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }}
    }}
}}";
                }
                else if (action == "Update")
                {
                    className = $"{action}{entityName}{kind}";
                    content = $@"
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Commands.Update
{{
    public class {className} : IRequestHandler<Update{entityName}Command, Unit>
    {{
        private readonly ITWJAppDbContext _context;

        public {className}(ITWJAppDbContext context)
        {{
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }}

        public async Task<Unit> Handle(Update{entityName}Command request, CancellationToken cancellationToken)
        {{
            var data = await _context.{entityName}.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            _context.{entityName}.Update(request.Update(data));

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
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
                    content = $@"
using FluentValidation;
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
                    return !await _context.{entityName}.AsNoTracking().AnyAsync(x => x.Property.ToLower() == name.ToLower(), cancellation);
                }}).WithMessage(x => ValidatorMessages.AlreadyExists($""Art with name {{x.Property}}""));
            }});
        }}
    }}
}}";
                }
                else if (action == "Update")
                {
                    className = $"{action}{entityName}{kind}";
                    content = $@"
using FluentValidation;
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
                    return await _context.{entityName}.AsNoTracking().AnyAsync(x => x.Property == id, cancellation);
                }}).WithMessage(ValidatorMessages.NotFound(""{entityName}"")).DependentRules(() =>
                {{
                    RuleFor(x => x.Property).MustAsync(async (args, id, cancellation) =>
                    {{
                        return !await _context.{entityName}.AsNoTracking().Where(x => x.Property != id).AnyAsync(x => x.Property == args.Property, cancellation);
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
                    content = $@"
using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetAll
{{
    public class {className} : IProfile
    {{
        public Guid Id {{ get; set; }}
        public string Property {{ get; set; }}

        public async Task MapData(IProfileMapper profileMapper)
        {{
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.{entityName}, {className}>(
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
                    content = $@"
using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetAll;
using TWJ.TWJApp.TWJService.Domain.Entities;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetById
{{
    public class {className} : IProfile
    {{
        public Guid Id {{ get; set; }}
        public string Name {{ get; set; }}

        public async Task MapData(IProfileMapper profileMapper)
        {{
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.{entityName}, GetAll{entityName}>(
                (src, options) =>
                {{
                    return new GetAll{entityName}
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
                    content = $@"
using MapperSegregator.Interfaces;
using System;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Domain.Entities.{entityName};

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetFiltered
{{
    public class {className} : IProfile
    {{
        public Guid Id {{ get; set; }}
        public string Property {{ get; set; }}

        public async Task MapData(IProfileMapper profileMapper)
        {{
            profileMapper.Build<TWJ.TWJApp.TWJService.Domain.Entities.{entityName}, {className}>(
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
                    content = $@"
using MediatR;
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
                    content = $@"
using MediatR;
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
                    content = $@"
using MediatR;
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
                    content = $@"
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetAll
{{
    public class {className} : IRequestHandler<GetAll{entityName}Query, IList<GetAll{entityName}Model>>
    {{
        private readonly ITWJAppDbContext _context;

        public {className}(ITWJAppDbContext context)
        {{
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }}

        public async Task<IList<GetAll{entityName}Model>> Handle(GetAll{entityName}Query request, CancellationToken cancellationToken)
        {{
            try
            {{
                var data = await _context.{entityName}
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
            catch (Exception e)
            {{
                throw e;
            }}
        }}
    }}
}}";
                }
                else if (action == "GetById")
                {
                    className = $"Get{entityName}ByIdQueryHandler";
                    content = $@"
using MapperSegregator.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Common.Constants;
using TWJ.TWJApp.TWJService.Common.Exceptions;
using TWJ.TWJApp.TWJService.Domain.Entities.{entityName};

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetById
{{
    public class {className} : IRequestHandler<Get{entityName}ByIdQuery, Get{entityName}ByIdModel>
    {{
        private readonly ITWJAppDbContext _context;
        private readonly IMapperSegregator _mapper;
        
        public {className}(ITWJAppDbContext context, IMapperSegregator mapper)
        {{
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }}

        public async Task<Get{entityName}ByIdModel> Handle(Get{entityName}ByIdQuery request, CancellationToken cancellationToken)
        {{
            var data = await _context.{entityName}.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (data == null) throw new BadRequestException(ValidatorMessages.NotFound(""Record""));

            return await _mapper.MapAsync<{entityName}Model, Get{entityName}ByIdModel>(data);
        }}
    }}
}}";
                }
                else if (action == "GetFiltered")
                {
                    className = $"GetFiltered{entityName}QueryHandler";
                    content = $@"
using Asp.Nappox.School.Common.Extensions;
using MapperSegregator.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TWJ.TWJApp.TWJService.Application.Dto.Models.{entityName};
using TWJ.TWJApp.TWJService.Application.Interfaces;

namespace TWJ.TWJApp.TWJService.Application.Services.{entityName}.Queries.GetFiltered
{{
public class {className} : IRequestHandler<GetFiltered{entityName}Query, FilterResponse<GetFiltered{entityName}Model>>
{{
    private readonly ITWJAppDbContext _context;
        
    public {className}(ITWJAppDbContext context)
    {{
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }}

    public async Task<FilterResponse<GetFiltered{entityName}Model>> Handle(GetFiltered{entityName}Query request, CancellationToken cancellationToken)
    {{
        try
        {{
            return new FilterResponse<GetFiltered{entityName}Model>
            {{
                Data = await _context.{entityName}
                        .AsNoTracking()
                        .OrderByDescending(x => x.Property)
                        .SkipAndTake(request.Page, request.PageSize, out int pages, out int items)
                        .OrderByDescending(x => x.Property)
                        .MapToListAsync<{entityName}Model, GetFiltered{entityName}Model>(),
                TotalPages = pages,
                TotalItems = items
            }};
        }}
        catch (Exception e)
        {{
            throw e;
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
