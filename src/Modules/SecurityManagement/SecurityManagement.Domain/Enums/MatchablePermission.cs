namespace SecurityManagement.Domain.Enums;

public enum MatchablePermission : uint
{
    SuperAdmin,

    // Department
    CanAccessDepartment,
    CreateDepartment,
    ReadDepartment,
    UpdateDepartment,
    DeleteDepartment,

    // Depot
    CanAccessDepot,
    CreateDepot,
    ReadDepot,
    UpdateDepot,
    DeleteDepot,

    // Designation
    CanAccessDesignation,
    CreateDesignation,
    ReadDesignation,
    UpdateDesignation,
    DeleteDesignation
}
