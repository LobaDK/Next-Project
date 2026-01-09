// UnitTests
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Reflection;
global using System.Security.Claims;
global using System.Threading.Tasks;

global using API.Controllers;
global using API.DTO.Requests.ActiveQuestionnaire;
global using API.DTO.Requests.Auth;
global using API.DTO.Requests.QuestionnaireTemplate;
global using API.DTO.Requests.User;
global using API.DTO.Responses.ActiveQuestionnaire;
global using API.DTO.Responses.Auth;
global using API.DTO.Responses.User;
global using API.DTO.User;
global using API.Exceptions;
global using API.Extensions;
global using API.FieldMappers;
global using API.Interfaces;
global using API.Services;

global using Database;
global using Database.DTO.ActiveQuestionnaire;
global using Database.DTO.ApplicationLog;
global using Database.DTO.QuestionnaireTemplate;
global using Database.DTO.User;
global using Database.Enums;
global using Database.Interfaces;
global using Database.Models;
global using Database.Repository;

global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Logging.Abstractions;

global using Moq;

global using Novell.Directory.Ldap;

global using Settings.Models;

global using Xunit;
