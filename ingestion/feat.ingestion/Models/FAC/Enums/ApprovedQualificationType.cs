using System.ComponentModel;

namespace feat.ingestion.Models.FAC.Enums;

public enum ApprovedQualificationType
{
    [Description("Access to Higher Education")]
    AccessToHigherEducation,

    [Description("Advanced Extension Award")]
    AdvancedExtensionAward,

    [Description("Alternative Academic Qualification")]
    AlternativeAcademicQualification,

    [Description("Digital Functional Skills Qualification")]
    DigitalFunctionalSkillsQualification,

    [Description("English For Speakers of Other Languages")]
    EnglishForSpeakersOfOtherLanguages,

    [Description("Essential Digital Skills")]
    EssentialDigitalSkills,

    [Description("Functional Skills")]
    FunctionalSkills,

    [Description("GCE A Level")]
    GCEAlevel,

    [Description("GCE AS Level")]
    GCEASLevel,

    [Description("GCSE (9 to 1)")]
    GCSE9To1,

    [Description("Occupational Qualification")]
    OccupationalQualification,

    [Description("Other General Qualification")]
    OtherGeneralQualification,

    [Description("Other Life Skills Qualification")]
    OtherLifeSkillsQualification,

    [Description("Other Vocational Qualification")]
    OtherVocationalQualification,

    [Description("Performing Arts Graded Examination")]
    PerformingArtsGradedExamination,

    [Description("Project")]
    Project,

    [Description("Technical Occupation Qualification")]
    TechnicalOccupationQualification,

    [Description("Technical Qualification")]
    TechnicalQualification,

    [Description("Vocationally-Related Qualification")]
    VocationallyRelatedQualification,

    [Description("T-Level")]
    TLevel,
    
    Unknown
}