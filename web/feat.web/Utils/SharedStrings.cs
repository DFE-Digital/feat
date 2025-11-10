namespace feat.web.Utils;

public static class SharedStrings
{
    // Commonly used string across web app.
    public const string AllClientFacets = "AllClientFacets";

    // Error Messages
    public const string LessThan100Char = "Please enter less than 100 characters";
    public const string SelectHowFarUCanTravel = "Please select how far you would be happy to travel.";
    public const string EnterValidLocation = "Please enter a valid location.";

    // RegEx patterns
    public const string PostcodePattern =
        "^([A-Za-z][A-Ha-hJ-Yj-y]?[0-9][A-Za-z0-9]? ?[0-9][A-Za-z]{2}|[Gg][Ii][Rr] ?0[Aa]{2})$";

    public static readonly string[] LocationsInEngland =
    [
        "","Abingdon", "Alnwick", "Altrincham", "Amersham", "Andover", "Ashford", "Ashby-de-la-Zouch", "Aylesbury",
        "Banbury", "Barnsley", "Barrow-in-Furness", "Basildon", "Basingstoke", "Bath", "Bedford", "Beverley",
        "Bicester", "Bideford", "Birmingham", "Blackburn", "Blackpool", "Bognor Regis", "Bolton", "Bootle",
        "Bournemouth", "Bracknell", "Bradford", "Braintree", "Brecon", "Brentwood", "Bridgwater", "Bridlington",
        "Bridport", "Brighton and Hove", "Bristol", "Bromley", "Burnley", "Burton upon Trent", "Bury",
        "Bury St Edmunds", "Cambridge", "Canterbury", "Carlisle", "Castleford", "Chatham", "Chelmsford", "Cheltenham",
        "Chester", "Chesterfield", "Chichester", "Chorley", "Clacton-on-Sea", "Colchester", "Corby", "Coventry",
        "Crewe", "Darlington", "Dartford", "Deal", "Derby", "Doncaster", "Dorchester", "Dover", "Dudley", "Durham",
        "Eastbourne", "Eastleigh", "Ebbw Vale", "Egham", "Ellesmere Port", "Ely", "Enfield", "Epsom", "Exeter",
        "Exmouth", "Falmouth", "Fareham", "Farnborough", "Farnham", "Felixstowe", "Fleet", "Folkestone", "Gateshead",
        "Gillingham", "Gloucester", "Grantham", "Gravesend", "Grays", "Great Malvern", "Great Yarmouth", "Grimsby",
        "Guildford", "Harlow", "Harrogate", "Hartlepool", "Hastings", "Hatfield", "Havant", "Hemel Hempstead",
        "Hereford", "Hertford", "High Wycombe", "Hinckley", "Huddersfield", "Hull", "Huntingdon", "Ilford", "Ipswich",
        "Isleworth", "Keighley", "Kendal", "Kidderminster", "King's Lynn", "Kingston upon Thames", "Knaresborough",
        "Lancaster", "Leamington Spa", "Leatherhead", "Leeds", "Leicester", "Lewes", "Lichfield", "Lincoln",
        "Littlehampton", "Liverpool", "London", "Loughborough", "Louth", "Luton", "Macclesfield", "Maidstone", "Maldon",
        "Malton", "Manchester", "Mansfield", "Margate", "Matlock", "Middlesbrough", "Milton Keynes", "Morecambe",
        "Newark-on-Trent", "Newcastle upon Tyne", "Newcastle-under-Lyme", "Newport (Isle of Wight)", "Northallerton",
        "Northampton", "Northwich", "Norwich", "Nottingham", "Nuneaton", "Oldham", "Ormskirk", "Oxford", "Paignton",
        "Penzance", "Peterborough", "Plymouth", "Poole", "Portsmouth", "Preston", "Reading", "Redcar", "Redditch",
        "Reigate", "Richmond", "Ripon", "Rochdale", "Rochester", "Rotherham", "Rugby", "Runcorn", "St Albans",
        "St Helens", "Salford", "Salisbury", "Scarborough", "Scunthorpe", "Sevenoaks", "Sheffield", "Shrewsbury",
        "Solihull", "South Shields", "Southampton", "Southend-on-Sea", "Southport", "Southsea", "Staines-upon-Thames",
        "Stafford", "Staines", "Stevenage", "Stockport", "Stockton-on-Tees", "Stoke-on-Trent", "Stratford-upon-Avon",
        "Sunderland", "Surbiton", "Sutton Coldfield", "Sutton-in-Ashfield", "Swansea", "Swindon", "Taunton", "Telford",
        "Thamesmead", "Thurrock", "Tiverton", "Torquay", "Truro", "Tunbridge Wells", "Uckfield", "Wakefield", "Walsall",
        "Warrington", "Warwick", "Watford", "Wellingborough", "Wells", "Welwyn Garden City", "West Bromwich",
        "Weston-super-Mare", "Weymouth", "Whitehaven", "Widnes", "Wigan", "Winchester", "Windsor", "Wirral", "Woking",
        "Wokingham", "Wolverhampton", "Worcester", "Workington", "Worthing", "Wrexham", "Yeovil", "York"
    ];

}