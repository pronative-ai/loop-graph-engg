// Project-wide using consolidation: all walkthroughs and helpers share these imports so
// individual files stay clean and focused. New source files in this project should
// avoid re-declaring these and instead rely on this global set.

global using Azure;
global using Azure.AI.OpenAI;
global using Azure.Identity;
global using OpenAI;
global using DotNetEnv;
global using Microsoft.Agents.AI;
global using Microsoft.Extensions.AI;
global using System.ClientModel;
global using System.Collections.Concurrent;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Text;
global using AgenticWorkflowConsole.Shared;