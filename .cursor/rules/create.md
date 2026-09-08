# PROJECT FAMILIARIZATION TASK

You have just opened this project/source folder.

Before doing ANY development work, first become familiar with this project by reviewing the existing project documentation.

IMPORTANT:
- Do NOT modify any source code.
- Do NOT create new functionality.
- Do NOT refactor anything.
- Do NOT modify database objects.
- Do NOT modify RDLC files.
- Do NOT modify frontend files.
- Do NOT modify configuration files.
- This task is ONLY for understanding and reviewing the project.

==================================================
1. REVIEW PROJECT DOCUMENTATION
==================================================

First check the following folder:

.cursor/doc/

Review ALL available Markdown documentation files, including:

- ARCHITECTURE.md
- FRONTEND.md
- BUSINESS_LOGIC.md
- RDLC_REPORTS.md
- DEVEXTREME.md
- CHANGE_LOG.md

Also review any other .md files present inside:

.cursor/doc/

Do not skip any relevant documentation.

==================================================
2. REVIEW THE ACTUAL PROJECT
==================================================

After reading the documentation, scan the actual project structure to verify your understanding.

Review:

- Project structure
- .NET Core 7.0 implementation
- Clean Architecture layers
- Controllers
- Services
- Repositories
- Models
- ViewModels
- Database access
- SQL / Stored Procedures where relevant
- Views
- HTML
- CSS
- JavaScript
- jQuery
- Bootstrap
- DevExtreme
- RDLC reports
- Configuration
- Existing common/shared functionality

Compare the documentation with the actual source code.

==================================================
3. CLIENT-SPECIFIC SOURCE
==================================================

This project may be a client-specific source/version of the main ERP project.

IMPORTANT:

Do NOT assume that another client's implementation is identical to this source.

Treat the currently opened source folder as the actual source of truth.

Identify:

- Client-specific modifications
- Client-specific business logic
- Client-specific screens
- Client-specific reports
- Client-specific database logic
- Client-specific frontend changes
- Client-specific DevExtreme implementations
- Client-specific configurations

Do not overwrite or normalize client-specific behavior based on documentation from another client.

==================================================
4. DOCUMENTATION VS CODE
==================================================

If the documentation and actual code differ:

- Trust the actual current source code.
- Do NOT modify the source code to match documentation.
- Do NOT automatically modify documentation during this familiarization task.

Instead, report the differences at the end.

==================================================
5. BUILD PROJECT UNDERSTANDING
==================================================

Before responding, internally understand:

1. How the project is structured.
2. Where business logic is located.
3. Where database operations are handled.
4. How frontend communicates with backend.
5. How DevExtreme grids/components are implemented.
6. How RDLC reports are generated and printed.
7. Where common/shared functionality exists.
8. Which areas appear to be client-specific.

You should use this understanding for all future tasks in this project.

==================================================
6. FUTURE DEVELOPMENT RULE
==================================================

From this point onward, when the user gives a development task:

- First understand the existing implementation.
- Follow the existing project architecture.
- Follow the existing coding patterns.
- Modify ONLY what is required.
- Modify ONLY the necessary files.
- Do NOT add unrequested functionality.
- Do NOT refactor unrelated code.
- Do NOT change unrelated business logic.
- Do NOT create unnecessary files.
- Do NOT introduce new frameworks or libraries unless explicitly requested.

Always treat the currently opened source folder as the source of truth.

==================================================
7. FINAL RESPONSE
==================================================

After reviewing everything, provide ONLY a short summary:

- Documentation reviewed
- Main architecture understood
- Main technologies identified
- Client-specific areas identified
- Any documentation/code mismatch found

Do not make any code changes.