## 1. Project Setup

- [x] 1.1 Create .NET 10 console application project structure with src/ directory
- [x] 1.2 Create .csproj file with required NuGet packages (Azure.Identity, Microsoft.Agents.AI, Microsoft.Agents.AI.Workflows)
- [x] 1.3 Create .gitignore file for .NET projects
- [x] 1.4 Create .aiignore file for AI assistant context
- [x] 1.5 Create Makefile with build, run, clean targets
- [x] 1.6 Create README.md with setup and usage instructions
- [x] 1.7 Create .env file with placeholder values for AKS_AGENT_GATEWAY_URL and AKS_AGENT_GATEWAY_KEY
- [x] 1.8 Create .env.example file documenting required environment variables

## 2. Core Workflow State

- [x] 2.1 Create CodingProjectState class with Goal, TasksCreated, IsApproved properties
- [x] 2.2 Implement state initialization and property accessors

## 3. Agent Implementation

- [x] 3.1 Create TerminalExecutionTool class implementing tool interface
- [x] 3.2 Implement terminal command execution logic
- [x] 3.3 Create BackendCoder agent with instructions and registered tool
- [x] 3.4 Create FrontendCoder agent with instructions

## 4. Workflow Graph

- [x] 4.1 Implement AgenticWorkflow graph with node registration
- [x] 4.2 Add Planner node with task creation logic
- [x] 4.3 Add BackendCoder and FrontendCoder agent nodes
- [x] 4.4 Add Reviewer node with approval logic
- [x] 4.5 Implement parallel split from Planner to Backend/Frontend
- [x] 4.6 Implement parallel join from Backend/Frontend to Reviewer
- [x] 4.7 Add conditional edge from Reviewer to Deployment
- [x] 4.8 Add terminal Deployment node

## 5. Middleware Guardrails

- [x] 5.1 Create HumanCheckpointStore class for approval management
- [x] 5.2 Implement TriggerApprovalPrompt method
- [x] 5.3 Implement WaitForApprovalAsync method
- [x] 5.4 Create deployment checkpoint middleware
- [x] 5.5 Register middleware on workflow graph

## 6. LLM Integration

- [x] 6.1 Load AKS_AGENT_GATEWAY_URL from environment variables
- [x] 6.2 Load AKS_AGENT_GATEWAY_KEY from environment variables
- [x] 6.3 Create AzureOpenAIClient configured for AKS agent gateway
- [x] 6.4 Create chat client for model inference

## 7. Application Entry Point

- [x] 7.1 Create Program.cs with async Main method
- [x] 7.2 Initialize workflow state with goal
- [x] 7.3 Execute workflow and handle exceptions
- [x] 7.4 Add error handling for missing environment variables
