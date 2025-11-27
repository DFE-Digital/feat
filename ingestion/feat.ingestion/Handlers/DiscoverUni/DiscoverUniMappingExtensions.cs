using feat.common.Models.Enums;
using feat.ingestion.Models.DU.Enums;
using NetTopologySuite.Geometries;
using DU = feat.ingestion.Models.DU;
using Location = feat.common.Models.Location;

namespace feat.ingestion.Handlers.DiscoverUni;

public static class DiscoverUniMappingExtensions
{
    public static Location ToLocation(this DU.Location l, DU.Institution? i)
    {
        var location = new Location
        {
            Created = DateTime.Now,
            Updated = DateTime.Now,
            Name = l.Name,
            GeoLocation = new Point(new Coordinate(l.Longitude, l.Latitude)) { SRID = 4326 },
            Url = l.StudentUnionUrl,
            SourceReference = $"{l.UKPRN}_{l.LocationId}",
            SourceSystem = SourceSystem.DiscoverUni
        };

        if (string.IsNullOrEmpty(i?.Address)) return location;

        var splitAddress = i?.Address?.Split(",").Select(s => s.Trim()).ToList();
        if (splitAddress == null || splitAddress.Count < 2)
        {
            return location;
        }

        location.Postcode = splitAddress[^1];
        location.Town = splitAddress[^2];

        switch (splitAddress.Count)
        {
            case 3:
                location.Address1 = splitAddress[0];
                break;
            case 4:
                location.Address1 = splitAddress[0];
                location.Address2 = splitAddress[1];
                break;
            case 5:
                location.Address1 = splitAddress[0];
                location.Address2 = splitAddress[1];
                location.Address3 = splitAddress[2];
                break;
            case 6:
                location.Address1 = splitAddress[0];
                location.Address2 = splitAddress[1];
                location.Address3 = splitAddress[2];
                location.Address4 = splitAddress[3];
                break;
        }

        return location;

    }

    public static CourseHours? ToCourseHours(this StudyMode studyMode)
    {
        return studyMode switch
        {
            StudyMode.FullTime => CourseHours.FullTime,
            StudyMode.PartTime => CourseHours.PartTime,
            StudyMode.Both => CourseHours.Flexible,
            _ => null
        };
    }

    public static LearningMethod? ToStudyMode(this Availability distanceLearningAvailability)
    {
        return distanceLearningAvailability switch
        {
            Availability.Compulsory => LearningMethod.Online,
            Availability.Optional => LearningMethod.Hybrid,
            Availability.NotAvailable => LearningMethod.ClassroomBased,
            _ => null
        };
    }

    public static QualificationLevel? ToQualificationLevel(this int aimCode)
    {
        return aimCode switch
        {
            0 => QualificationLevel.Level6, // BA - Bachelor of Arts
            1 => QualificationLevel.Level6, // BAcc - Bachelor of Accountancy
            2 => QualificationLevel.Level6, // BArch - Bachelor of Architecture
            3 => QualificationLevel.Level6, // BBA - Bachelor of Business Administration
            4 => QualificationLevel.Level6, // BD - Bachelor of Divinity
            5 => QualificationLevel.Level6, // BDiv - Bachelor of Divinity
            6 => QualificationLevel.Level6, // BDS - Bachelor of Dental Surgery
            7 => QualificationLevel.Level6, // BEd - Bachelor of Education
            8 => QualificationLevel.Level6, // BEng - Bachelor of Engineering
            9 => QualificationLevel.Level6, // BFin - Bachelor of Finance
            10 => QualificationLevel.Level6, // BHSc - Bachelor of Health Sciences
            11 => QualificationLevel.Level6, // BM - Bachelor of Medicine
            12 => QualificationLevel.Level6, // BMedSc - Bachelor of Medical Science
            13 => QualificationLevel.Level6, // BMedSci - Bachelor of Medical Sciences
            14 => QualificationLevel.Level6, // BMid - Bachelor of Midwifery
            15 => QualificationLevel.Level6, // BMidWif - Bachelor of Midwifery
            16 => QualificationLevel.Level6, // BMus - Bachelor of Music
            17 => QualificationLevel.Level6, // BN - Bachelor of Nursing
            18 => QualificationLevel.Level6, // BNurs - Bachelor of Nursing
            19 => QualificationLevel.Level6, // BOptom - Bachelor of Optometry
            20 => QualificationLevel.Level6, // BOst - Bachelor of Osteopathy
            21 => QualificationLevel.Level6, // BSc - Bachelor of Science
            22 => QualificationLevel.Level6, // BSW - Bachelor of Social Work
            23 => QualificationLevel.Level6, // BTechEd - Bachelor of Technology Education
            24 => QualificationLevel.Level6, // BTh - Bachelor of Theology
            25 => QualificationLevel.Level6, // BVetMed - Bachelor of Veterinary Medicine
            26 => QualificationLevel.Level6, // BVMS - Bachelor of Veterinary Medicine & Surgery
            27 => QualificationLevel.Level6, // BVSc - Bachelor of Veterinary Science
            28 => QualificationLevel.Level4, // CertHE - Certificate of Higher Education
            29 => QualificationLevel.Level5, // DipHE - Diploma of Higher Education
            30 => QualificationLevel.Level5, // FD - Foundation Degree
            31 => QualificationLevel.Level5, // FDA - Foundation Degree in Arts
            32 => QualificationLevel.Level5, // FDArts - Foundation Degree in Arts
            33 => QualificationLevel.Level5, // FDEd - Foundation Degree in Education
            34 => QualificationLevel.Level5, // FDEng - Foundation Degree in Engineering
            35 => QualificationLevel.Level5, // FDS - Foundation Degree in Science
            36 => QualificationLevel.Level5, // FdSc - Foundation Degree in Science
            37 => QualificationLevel.Level5, // HND - Higher National Diploma
            38 => QualificationLevel.Level6, // LLB - Bachelor of Laws
            39 => QualificationLevel.Level7, // MA - Master of Arts
            40 => QualificationLevel.Level7, // MArch - Master of Architecture
            41 => QualificationLevel.Level7, // MART - Master of Art
            42 => QualificationLevel.Level6, // MBBS - Bachelor of Medicine & Bachelor of Surgery
            43 => QualificationLevel.Level6, // MBBCh - Bachelor of Medicine & Bachelor of Surgery
            44 => QualificationLevel.Level6, // MBChB - Bachelor of Medicine & Bachelor of Surgery
            45 => QualificationLevel.Level7, // MBiol - Master of Biology
            46 => QualificationLevel.Level7, // MBiolSci - Master of Biological Science
            47 => QualificationLevel.Level7, // MBioMed Sci - Master of Biomedical Science
            48 => QualificationLevel.Level7, // MChem - Master of Chemistry
            49 => QualificationLevel.Level7, // MChiro - Master of Chiropractic
            50 => QualificationLevel.Level7, // MClinRes - Master of Clinical Research
            51 => QualificationLevel.Level7, // MDSci - Master of Dental Science
            52 => QualificationLevel.Level7, // MEarthSci - Master of Earth Sciences
            53 => QualificationLevel.Level7, // MEd - Master of Education
            54 => QualificationLevel.Level7, // MEng - Master of Engineering
            55 => QualificationLevel.Level7, // MEnvSci - Master of Environmental Science
            56 => QualificationLevel.Level7, // MGeol - Master of Geology
            57 => QualificationLevel.Level7, // MMathStat - Master of Mathematics and Statistics
            58 => QualificationLevel.Level7, // MMORSE - Master of Mathematics, Operational Research, Stats & Econ
            59 => QualificationLevel.Level7, // MMus - Master of Music
            60 => QualificationLevel.Level7, // MOst - Master of Osteopathy
            61 => QualificationLevel.Level7, // MPharm - Master of Pharmacy
            62 => QualificationLevel.Level7, // MPhys - Master of Physics
            63 => QualificationLevel.Level7, // MMarBiol - Master of Marine Biology
            64 => QualificationLevel.Level7, // MMath - Master of Mathematics
            65 => QualificationLevel.Level7, // MMBiol - Master of Molecular Biology
            66 => QualificationLevel.Level7, // MME - Master of Mechanical Engineering
            67 => QualificationLevel.Level7, // MMet - Master of Meteorology
            68 => QualificationLevel.Level7, // MMSci - Master of Marine Science
            69 => QualificationLevel.Level7, // MMathPhys - Master of Mathematics and Physics
            70 => QualificationLevel.Level7, // MNatSc - Master of Natural Sciences
            71 => QualificationLevel.Level7, // MPH - Master of Public Health
            72 => QualificationLevel.Level7, // MPsych - Master of Psychology
            73 => QualificationLevel.Level7, // MSci - Master in Science
            74 => QualificationLevel.Level7, // MSt - Master of Studies
            75 => QualificationLevel.Level7, // MTh - Master of Theology
            76 => QualificationLevel.Level7, // MRes - Master of Research
            77 => QualificationLevel.Level7, // MEngD - Engineering Doctorate (taught component at L7; overall L8)
            78 => QualificationLevel.Level7, // TQFE - Teaching Qualification, Further Education
            79 => QualificationLevel.Level7, // MAnth - Master of Anthropology
            80 => QualificationLevel.Level7, // MPhEd - Master of Physical Education
            81 => QualificationLevel.Level7, // MPhysPhil - Master of Physics and Philosophy
            82 => QualificationLevel.Level7, // MMathPhysPhil - Master of Mathematics, Physics and Philosophy
            83 => QualificationLevel.Level7, // MEngPhil - Master of Engineering and Philosophy
            84 => QualificationLevel.Level7, // MMathPhil - Master of Mathematics and Philosophy
            85 => QualificationLevel.Level6, // DipArch - Diploma of Architecture
            86 => QualificationLevel.Level7, // MMetOce - Master of Meteorology and Oceanography
            87 => QualificationLevel.Level7, // DipArch(Cons) - Diploma of Architecture (Conservation) -> treat as L7? (if advanced) else L6
            88 => QualificationLevel.Level7, // MStat - Master of Statistics
            89 => QualificationLevel.Level7, // MPhysPhil - Master of Physics and Philosophy
            90 => QualificationLevel.Level7, // MPhilStud - Master of Philosophy (Stud.)
            91 => QualificationLevel.Level7, // MFin - Master of Finance
            92 => QualificationLevel.Level7, // MDataSci - Master of Data Science
            93 => QualificationLevel.Level7, // MMathCompSci - Master of Mathematics and Computer Science
            94 => QualificationLevel.Level7, // MMathPhil - Master of Mathematics and Philosophy
            95 => QualificationLevel.Level7, // MChemPhys - Master of Chemistry and Physics
            96 => QualificationLevel.Level7, // MCompSci - Master of Computer Science
            97 => QualificationLevel.Level7, // MEngCompSci - Master of Engineering and Computer Science
            98 => QualificationLevel.Level7, // MArtsSci - Master of Arts and Sciences
            99 => QualificationLevel.Level7, // MInf - Master of Informatics
            100 => QualificationLevel.Level7, // MMathComp - Master of Mathematics and Computing
            101 => QualificationLevel.Level7, // MMathCompPhil - Master of Mathematics, Computation & Philosophy
            102 => QualificationLevel.Level7, // MPhysAstro - Master of Physics and Astrophysics
            103 => QualificationLevel.Level7, // MPhysMath - Master of Physics and Mathematics
            104 => QualificationLevel.Level7, // MPhysTheor - Master of Physics (Theoretical)
            105 => QualificationLevel.Level7, // MMathPhilStat - Master of Mathematics, Philosophy & Statistics
            106 => QualificationLevel.Level7, // MEngPhys - Master of Engineering and Physics
            107 => QualificationLevel.Level7, // MMath - Master of Mathematics (variant)
            108 => QualificationLevel.Level7, // MSci(Chem) - MSci Chemistry
            109 => QualificationLevel.Level7, // MSci(Phys) - MSci Physics
            110 => QualificationLevel.Level7, // MSci(Biol) - MSci Biology
            111 => QualificationLevel.Level7, // IPML - Integrated Professional Master in Languages
            112 => QualificationLevel.Level8, // PhD - Doctor of Philosophy
            113 => QualificationLevel.Level7, // IPML (dup/variant) - Integrated Prof Master in Languages
            114 => QualificationLevel.Level8, // DPhil - Doctor of Philosophy
            115 => QualificationLevel.Level7, // MMath&Phys - Master of Mathematics and Physics
            116 => QualificationLevel.Level8, // EdD - Doctor of Education
            117 => QualificationLevel.Level7, // MMath&Phys (variant) - Master of Mathematics and Physics
            118 => QualificationLevel.Level7, // MPhys&Phil (variant) - Master of Physics and Philosophy
            119 => QualificationLevel.Level7, // MMathPhys (variant)
            120 => QualificationLevel.Level7, // MPhys (variant)
            121 => QualificationLevel.Level7, // MEng (variant)
            122 => QualificationLevel.Level7, // MBiol (variant)
            123 => QualificationLevel.Level7, // MMkt - Master in Marketing
            124 => QualificationLevel.Level6, // Graduate Diploma - (HE) treated as Level 6
            125 => QualificationLevel.Level7, // MSc - Master of Science
            126 => QualificationLevel.Level7, // Grad Dip (variant naming)
            127 => QualificationLevel.Level8, // EngD - Engineering Doctorate
            128 => QualificationLevel.Level7, // MTheol - Master of Theology
            129 => QualificationLevel.Level6, // Vet.M.B - Bachelor of Veterinary Medicine
            130 => QualificationLevel.Level7, // MLitt - Master of Letters
            131 => QualificationLevel.Level6, // Vet.M.B (variant) - Bachelor of Veterinary Medicine
            132 => QualificationLevel.Level7, // MPH (variant) - Master of Public Health
            133 => QualificationLevel.Level7, // MPA - Master of Public Administration
            134 => QualificationLevel.Level7, // MPP - Master of Public Policy
            135 => QualificationLevel.Level7, // MDes - Master of Design
            136 => QualificationLevel.Level7, // MClinDent - Master of Clinical Dentistry
            137 => QualificationLevel.Level7, // MEd (variant)
            138 => QualificationLevel.Level7, // MRes (variant)
            139 => QualificationLevel.Level7, // MEnt - Master of Enterprise
            140 => QualificationLevel.Level7, // MEntSci - Master of Enterprise in Science
            141 => QualificationLevel.Level7, // MArch(Cons) - Master of Architecture (Conservation)
            142 => QualificationLevel.Level7, // MArch(Des) - Master of Architecture (Design)
            143 => QualificationLevel.Level7, // MArch(Interiors) - Master of Architecture (Interiors)
            144 => QualificationLevel.Level7, // MMathCompSci (variant)
            145 => QualificationLevel.Level7, // MPhil - Master of Philosophy (taught)
            146 => QualificationLevel.Level7, // MPhilRes - MPhil (by Research) – often considered L7
            147 => QualificationLevel.Level7, // MVM - Master of Veterinary Medicine
            148 => QualificationLevel.Level7, // MVS - Master of Veterinary Science
            149 => QualificationLevel.Level7, // MEdPsych - Master of Educational Psychology
            150 => QualificationLevel.Level7, // MMedSci - Master of Medical Science
            151 => QualificationLevel.Level7, // MResMedSci - Master of Research (Medical Science)
            152 => QualificationLevel.Level7, // MA(Res) - Master of Arts (Research)
            153 => QualificationLevel.Level7, // MSc(Res) - Master of Science (Research)
            154 => QualificationLevel.Level7, // MClinRes (variant)
            155 => QualificationLevel.Level7, // MMedSci (variant) - Masters of Medical Science
            156 => QualificationLevel.Level7, // MArts(Res) - Master of Arts (Research)
            157 => QualificationLevel.Level7, // MInf(Res) - Master of Informatics (Research)
            158 => QualificationLevel.Level7, // MMus(Res) - Master of Music (Research)
            159 => QualificationLevel.Level7, // MDes(Res) - Master of Design (Research)
            160 => QualificationLevel.Level7, // MLitt(Res) - Master of Letters (Research)
            161 => QualificationLevel.Level7, // MTheol(Res) - Master of Theology (Research)
            162 => QualificationLevel.Level7, // MPhilStud (variant)
            163 => QualificationLevel.Level7, // MClinEd - Master of Clinical Education
            164 => QualificationLevel.Level7, // MRes(Clin) - Master of Research (Clinical)
            165 => QualificationLevel.Level7, // MMORSE (variant) - Master of Maths, OR, Stats & Econ
            166 => QualificationLevel.Level7, // MMathPhys (variant)
            167 => QualificationLevel.Level7, // MPsych(Clin) - Master of Psychology (Clinical)
            168 => QualificationLevel.Level7, // MComp - Master of Computing
            169 => QualificationLevel.Level7, // MMus (variant) - Master of Music
            170 => QualificationLevel.Level7, // MPhil(CompSci) - MPhil in Computer Science
            171 => QualificationLevel.Level7, // MPhil(Econ) - MPhil in Economics
            172 => QualificationLevel.Level7, // MPhil(Stats) - MPhil in Statistics
            173 => QualificationLevel.Level7, // MPhil(Eng) - MPhil in Engineering
            174 => QualificationLevel.Level7, // MPhil(SocSci) - MPhil in Social Sciences
            175 => QualificationLevel.Level7, // MPhil(Hum) - MPhil in Humanities
            176 => QualificationLevel.Level6, // Advanced Diploma - HE Advanced Diploma (treated L6)
            177 => QualificationLevel.Level7, // MEd(Music) - Master in Music Education
            178 => QualificationLevel.Level7, // MRes(Arts) - Master of Research in Arts
            179 => QualificationLevel.Level7, // MRes(Science) - Master of Research in Science
            180 => QualificationLevel.Level7, // MRes(Eng) - Master of Research in Engineering
            181 => QualificationLevel.Level7, // MRes(SocSci) - Master of Research in Social Science
            182 => QualificationLevel.Level7, // MRes(Health) - Master of Research in Health
            183 => QualificationLevel.Level7, // MRes(BioSci) - Master of Research in Biological Sciences
            184 => QualificationLevel.Level7, // MRes(MedSci) - Master of Research in Medical Sciences
            185 => QualificationLevel.Level7, // MRes(Psych) - Master of Research in Psychology
            186 => QualificationLevel.Level7, // MRes(Chem) - Master of Research in Chemistry
            187 => QualificationLevel.Level7, // MRes(Phys) - Master of Research in Physics
            188 => QualificationLevel.Level7, // MRes(Geol) - Master of Research in Geology
            189 => QualificationLevel.Level4, // LicAc - Licentiate in Acupuncture
            190 => QualificationLevel.Level7, // MRes(ClinSc) - Master of Research in Clinical Sciences
            191 => QualificationLevel.Level7, // MRes(CompSci) - Master of Research in Computer Science
            192 => QualificationLevel.Level4, // LicAc (variant) - Licentiate in Acupuncture
            193 => QualificationLevel.Level7, // MRes(EnvSci) - Master of Research in Environmental Science
            194 => QualificationLevel.Level7, // MRes(EarthSci) - Master of Research in Earth Sciences
            195 => QualificationLevel.Level7, // MRes(LifeSci) - Master of Research in Life Sciences
            196 => QualificationLevel.Level7, // MRes(NatSci) - Master of Research in Natural Sciences
            197 => QualificationLevel.Level7, // MRes(Stats) - Master of Research in Statistics
            198 => QualificationLevel.Level7, // MRes(CompEng) - Master of Research in Computer Engineering
            199 => QualificationLevel.Level7, // MRes(Neuro) - Master of Research in Neuroscience
            200 => QualificationLevel.Level7, // MRes(InfoSci) - Master of Research in Information Science
            201 => QualificationLevel.Level7, // MRes(Design) - Master of Research in Design
            202 => QualificationLevel.Level7, // MRes(MusTech) - Master of Research in Music Technology
            _ => null
        };
    }
}




    
