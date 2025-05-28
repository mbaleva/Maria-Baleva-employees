## Project Overview

This project identifies the **pair of employees who have worked together on common projects for the longest period of time**. It includes:

- A backend API built with **ASP.NET Core** (`Sirma` solution)
- A frontend UI built with **React** (`employee-overlap-ui`)

The application accepts a CSV file with employee project data and returns results showing which employees collaborated the most, by overlapping project days.

The service uses a project-based grouping algorithm to identify employee pairs who worked together by comparing their overlapping date ranges only within the same project. 
This approach reduces unnecessary comparisons across unrelated records, improving efficiency compared to naive methods and making the solution scalable to large datasets.

