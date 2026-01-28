// API
global using System.Collections;
global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics;
global using System.IdentityModel.Tokens.Jwt;
global using System.Net;
global using System.Net.WebSockets;
global using System.Reflection;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;

global using API.Attributes;
global using API.DTO.LDAP;
global using API.DTO.Requests.ActiveQuestionnaire;
global using API.DTO.Requests.Auth;
global using API.DTO.Requests.QuestionnaireTemplate;
global using API.DTO.Requests.Settings;
global using API.DTO.Requests.User;
global using API.DTO.Responses.ActiveQuestionnaire;
global using API.DTO.Responses.Auth;
global using API.DTO.Responses.QuestionnaireTemplate;
global using API.DTO.Responses.Settings;
global using API.DTO.Responses.Settings.SettingsSchema;
global using API.DTO.Responses.Settings.SettingsSchema.Bases;
global using API.DTO.Responses.User;
global using API.DTO.User;
global using API.Enums;
global using API.Exceptions;
global using API.Extensions;
global using API.FieldMappers;
global using API.Interfaces;
global using API.Mock;
global using API.Services;
global using API.Services.Authentication;
global using API.Utils;
global using API.Validators;
global using static API.Exceptions.LDAPException;

global using Database;
global using API.Mock;
global using API.Services;
global using API.Services.Authentication;
global using API.Utils;
global using API.Validators;

global using Database;
global using Database.DTO.ActiveQuestionnaire;
global using Database.DTO.ApplicationLog;
global using Database.DTO.QuestionnaireTemplate;
global using Database.DTO.User;
global using Database.Enums;
global using Database.Extensions;
global using Database.Interfaces;
global using Database.Models;
global using Database.Repository;

global using FluentValidation;
global using FluentValidation.AspNetCore;

global using Logging.Extensions;
global using Logging.LogEvents;

global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.ApplicationModels;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Storage;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.Net.Http.Headers;
global using Microsoft.OpenApi.Models;

global using Novell.Directory.Ldap;
global using Novell.Directory.Ldap.Controls;

global using Serilog;

global using Settings.Interfaces;
global using Settings.Models;
