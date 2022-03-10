using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Servicios.api.Seguridad.Core.DTO;
using Servicios.api.Seguridad.Core.Entities;
using Servicios.api.Seguridad.Core.JwtLogic;
using Servicios.api.Seguridad.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios.api.Seguridad.Core.Application
{
    public class Login
    {
        public class UsuarioLoginCommand : IRequest<UsuarioDTO>
        {
            public string Email { get; set; }
            public string Password { get; set; }

            public class UsuarioLoginValidation : AbstractValidator<UsuarioLoginCommand>
            {
                public UsuarioLoginValidation()
                {
                    RuleFor(x => x.Email).NotEmpty();
                    RuleFor(x => x.Password).NotEmpty();
                }
            }

            public class UsuarioLoginHanlder : IRequestHandler<UsuarioLoginCommand, UsuarioDTO>
            {
                private readonly SeguridadContexto _context;
                private readonly UserManager<Usuario> _userManager;
                private IMapper _mapper;
                private readonly IJwtGenerator _jwtGenerator;
                private readonly SignInManager<Usuario> _signInManager;
                public UsuarioLoginHanlder(SeguridadContexto context, UserManager<Usuario> userManager, IMapper mapper, IJwtGenerator jwtGenerator, SignInManager<Usuario> signInManager)
                {
                    _context = context;
                    _userManager = userManager;
                    _mapper = mapper;
                    _jwtGenerator = jwtGenerator;
                    _signInManager = signInManager;
                }
                public async Task<UsuarioDTO> Handle(UsuarioLoginCommand request, CancellationToken cancellationToken)
                {
                    var usuario = await _userManager.FindByEmailAsync(request.Email);
                    if(usuario == null)
                    {
                        throw new Exception("El usuario no existe");
                    }

                    var result = await _signInManager.CheckPasswordSignInAsync(usuario, request.Password, false);
                    if (result.Succeeded)
                    {
                        var usuarioDto = _mapper.Map<Usuario, UsuarioDTO>(usuario);
                        usuarioDto.Token = _jwtGenerator.CreateToken(usuario);
                        return usuarioDto;
                    }

                    throw new Exception("Login incorrecto");
                }
            }
        }
    }
}
