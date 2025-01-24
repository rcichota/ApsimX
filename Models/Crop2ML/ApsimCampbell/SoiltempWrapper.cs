using APSIM.Shared.Utilities;
using Models.Climate;
using Models.Core;
using Models.Interfaces;
using Models.PMF;
using Models.Soils;
using Models.Surface;
using Models.WaterModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Models.Factorial;

namespace Models.Crop2ML.ApsimCampbell;

/// <summary>
///  This class encapsulates the SoiltempComponent
/// </summary>
[Serializable]
[PresenterName("UserInterface.Presenters.PropertyPresenter")]
[ViewName("UserInterface.Views.PropertyView")]
[ValidParent(ParentType = typeof(Zone))]
[ValidParent(ParentType = typeof(Factor))]
class SoiltempWrapper :  Model
{
    [Link] Clock clock = null;
    [Link] Weather weather = null;
    [Link] Physical physical = null;
    [Link] Organic organic = null;
//    [Link] Water water = null;
    [Link] WaterBalance waterBalance = null;
    [Link] MicroClimate microClimate = null;
    [Link] Simulation simulation = null;

    private SoiltempState s;
    private SoiltempState s1;
    private SoiltempRate r;
    private SoiltempAuxiliary a;
    private SoiltempExogenous ex;
    private SoiltempComponent soiltempComponent;

    /// <summary>
    ///  The constructor of the Wrapper of the SoiltempComponent
    /// </summary>
    public SoiltempWrapper()
    {
        s = new SoiltempState();
        s1 = new SoiltempState();
        r = new SoiltempRate();
        a = new SoiltempAuxiliary();
        ex = new SoiltempExogenous();
        soiltempComponent = new SoiltempComponent();
    }

    /// <summary>
    ///  The get method of the Net radiation per internal time-step output variable
    /// </summary>
    [Description("Net radiation per internal time-step")]
    [Units("MJ")]
    public double netRadiation{ get { return s.netRadiation;}} 
     

    /// <summary>
    ///  The get method of the Internal time-step output variable
    /// </summary>
    [Description("Internal time-step")]
    [Units("sec")]
    public double internalTimeStep{ get { return s.internalTimeStep;}} 
     

    /// <summary>
    ///  The get method of the K, conductance of element between nodes output variable
    /// </summary>
    [Description("K, conductance of element between nodes")]
    [Units("W/K")]
    public double[] thermalConductance{ get { return s.thermalConductance;}} 
     

    /// <summary>
    ///  The get method of the Flag whether initialisation is needed output variable
    /// </summary>
    [Description("Flag whether initialisation is needed")]
    [Units("mintes")]
    public bool doInitialisationStuff{ get { return s.doInitialisationStuff;}} 
     

    /// <summary>
    ///  The get method of the Yesterday's maximum daily air temperature output variable
    /// </summary>
    [Description("Yesterday's maximum daily air temperature")]
    [Units("oC")]
    public double maxTempYesterday{ get { return s.maxTempYesterday;}} 
     

    /// <summary>
    ///  The get method of the Time of day from midnight output variable
    /// </summary>
    [Description("Time of day from midnight")]
    [Units("sec")]
    public double timeOfDaySecs{ get { return s.timeOfDaySecs;}} 
     

    /// <summary>
    ///  The get method of the Volumetric water content of each soil layer output variable
    /// </summary>
    [Description("Volumetric water content of each soil layer")]
    [Units("mm3/mm3")]
    public double[] soilWater{ get { return s.soilWater;}} 
     

    /// <summary>
    ///  The get method of the Soil temperature over the soil profile at morning output variable
    /// </summary>
    [Description("Soil temperature over the soil profile at morning")]
    [Units("oC")]
    public double[] soilTemp{ get { return s.soilTemp;}} 
     

    /// <summary>
    ///  The get method of the Height of instruments above soil surface output variable
    /// </summary>
    [Description("Height of instruments above soil surface")]
    [Units("mm")]
    public double instrumentHeight{ get { return s.instrumentHeight;}} 
     

    /// <summary>
    ///  The get method of the Volumetric specific heat over the soil profile output variable
    /// </summary>
    [Description("Volumetric specific heat over the soil profile")]
    [Units("J/K/m3")]
    public double[] volSpecHeatSoil{ get { return s.volSpecHeatSoil;}} 
     

    /// <summary>
    ///  The get method of the Height of canopy above ground output variable
    /// </summary>
    [Description("Height of canopy above ground")]
    [Units("mm")]
    public double canopyHeight{ get { return s.canopyHeight;}} 
     

    /// <summary>
    ///  The get method of the CP, heat storage between nodes output variable
    /// </summary>
    [Description("CP, heat storage between nodes")]
    [Units("J/K")]
    public double[] heatStorage{ get { return s.heatStorage;}} 
     

    /// <summary>
    ///  The get method of the Minimum soil temperature output variable
    /// </summary>
    [Description("Minimum soil temperature")]
    [Units("oC")]
    public double[] minSoilTemp{ get { return s.minSoilTemp;}} 
     

    /// <summary>
    ///  The get method of the Maximum soil temperature output variable
    /// </summary>
    [Description("Maximum soil temperature")]
    [Units("oC")]
    public double[] maxSoilTemp{ get { return s.maxSoilTemp;}} 
     

    /// <summary>
    ///  The get method of the Soil temperature at the end of this iteration output variable
    /// </summary>
    [Description("Soil temperature at the end of this iteration")]
    [Units("oC")]
    public double[] newTemperature{ get { return s.newTemperature;}} 
     

    /// <summary>
    ///  The get method of the Air temperature output variable
    /// </summary>
    [Description("Air temperature")]
    [Units("oC")]
    public double airTemperature{ get { return s.airTemperature;}} 
     

    /// <summary>
    ///  The get method of the Thermal conductivity output variable
    /// </summary>
    [Description("Thermal conductivity")]
    [Units("W.m/K")]
    public double[] thermalConductivity{ get { return s.thermalConductivity;}} 
     

    /// <summary>
    ///  The get method of the Yesterday's minimum daily air temperature output variable
    /// </summary>
    [Description("Yesterday's minimum daily air temperature")]
    [Units("oC")]
    public double minTempYesterday{ get { return s.minTempYesterday;}} 
     

    /// <summary>
    ///  The get method of the Initial soil temperature output variable
    /// </summary>
    [Description("Initial soil temperature")]
    [Units("oC")]
    public double[] InitialValues{ get { return s.InitialValues;}} 
     

    /// <summary>
    ///  The get method of the Average daily atmosphere boundary layer conductance output variable
    /// </summary>
    [Description("Average daily atmosphere boundary layer conductance")]
    [Units("")]
    public double boundaryLayerConductance{ get { return s.boundaryLayerConductance;}} 
     

    /// <summary>
    ///  The get method of the average soil temperature output variable
    /// </summary>
    [Description("average soil temperature")]
    [Units("oC")]
    public double[] aveSoilTemp{ get { return s.aveSoilTemp;}} 
     

    /// <summary>
    ///  The get method of the Soil temperature over the soil profile at morning output variable
    /// </summary>
    [Description("Soil temperature over the soil profile at morning")]
    [Units("oC")]
    public double[] morningSoilTemp{ get { return s.morningSoilTemp;}} 
     

    /// <summary>
    ///  The Constructor copy of the wrapper of the SoiltempComponent
    /// </summary>
    /// <param name="toCopy"></param>
    /// <param name="copyAll"></param>
    public SoiltempWrapper(SoiltempWrapper toCopy, bool copyAll) 
    {
        s = (toCopy.s != null) ? new SoiltempState(toCopy.s, copyAll) : null;
        r = (toCopy.r != null) ? new SoiltempRate(toCopy.r, copyAll) : null;
        a = (toCopy.a != null) ? new SoiltempAuxiliary(toCopy.a, copyAll) : null;
        ex = (toCopy.ex != null) ? new SoiltempExogenous(toCopy.ex, copyAll) : null;
        if (copyAll)
        {
            soiltempComponent = (toCopy.soiltempComponent != null) ? new SoiltempComponent(toCopy.soiltempComponent) : null;
        }
    }

    /// <summary>
    ///  The Initialization method of the wrapper of the SoiltempComponent
    /// </summary>
    public void Init(){
        setExogenous();
        loadParameters();
        soiltempComponent.Init(s, s1, r, a, ex);
    }

    /// <summary>
    ///  Load parameters of the wrapper of the SoiltempComponent
    /// </summary>
    private void loadParameters()
    {
//        soiltempComponent.thermCondPar1 = null; // To be modified
        soiltempComponent.topsoilNode = 2; 
        soiltempComponent.surfaceNode = 1; 
        soiltempComponent.numPhantomNodes = 5; 
        soiltempComponent.soilConstituentNames = new string[] { "Rocks", "OrganicMatter", "Sand", "Silt", "Clay", "Water", "Ice", "Air"};
        soiltempComponent.physical_Thickness = physical.Thickness;
        soiltempComponent.MissingValue = 99999; 
        soiltempComponent.timestep = 24.0 * 60.0 * 60.0; 
//        soiltempComponent.soilRoughnessHeight = null; // To be modified
        soiltempComponent.numIterationsForBoundaryLayerConductance = 1; 
        soiltempComponent.defaultTimeOfMaximumTemperature = 14.0;
        soiltempComponent.pom = 1.3;
        soiltempComponent.DepthToConstantTemperature = 10000; 
        soiltempComponent.constantBoundaryLayerConductance = 20; 
//        soiltempComponent.thermCondPar4 = null; // To be modified
//        soiltempComponent.nodeDepth = null; // To be modified
        soiltempComponent.nu = 0.6;
//        double[] initialTempValues = new double[physical.Thickness.Length];
//        Array.Fill(initialTempValues, weather.Tav);
//        soiltempComponent.InitialValues = initialTempValues;
        soiltempComponent.pInitialValues = null; // To be modified
        soiltempComponent.ps = 2.63;
        soiltempComponent.netRadiationSource = "calc";
        soiltempComponent.airNode = 0; 
        soiltempComponent.bareSoilRoughness = 57; 
//        soiltempComponent.thermCondPar2 = null; // To be modified
        soiltempComponent.defaultInstrumentHeight = 1.2;
        soiltempComponent.physical_BD = physical.BD;
        soiltempComponent.latentHeatOfVapourisation = 2465000; 
        soiltempComponent.weather_Latitude = weather.Latitude;
        soiltempComponent.stefanBoltzmannConstant = 0.0000000567;
        soiltempComponent.boundarLayerConductanceSource = "calc";
//        soiltempComponent.thermCondPar3 = null; // To be modified
    }

    /// <summary>
    ///  Set exogenous variables of the wrapper of the SoiltempComponent
    /// </summary>
    private void setExogenous()
    {
        ex.waterBalance_Eo = (double)simulation.Get("[ReadWTHFile].Script.PotEvapotranspiration"); //should be: waterBalance.Eo;
        ex.waterBalance_Salb = waterBalance.Salb;
        ex.organic_Carbon = organic.Carbon;
        ex.waterBalance_Es = (double)simulation.Get("[ReadWTHFile].Script.ActualEvaporation");  //should be: waterBalance.Es;
        ex.weather_Wind = weather.Wind;
        ex.physical_ParticleSizeSand = physical.ParticleSizeSand;
        ex.weather_AirPressure = weather.AirPressure;
        ex.clock_Today_DayOfYear = clock.Today.DayOfYear;
        ex.microClimate_CanopyHeight = microClimate.CanopyHeight;
        ex.waterBalance_Eos = (double)simulation.Get("[ReadWTHFile].Script.PotEvaporation");  //should be: waterBalance.Eos;
        ex.waterBalance_SW = waterBalance.SW;
        ex.weather_Amp = weather.Amp;
        ex.weather_MinT = weather.MinT;
        ex.weather_Radn = weather.Radn;
        ex.physical_Rocks = physical.Rocks;
        ex.weather_Tav = weather.Tav;
        ex.weather_MaxT = weather.MaxT;
        ex.weather_MeanT = weather.MeanT;
        ex.physical_ParticleSizeSilt = physical.ParticleSizeSilt;
        ex.physical_ParticleSizeClay = physical.ParticleSizeClay;
    }

    [EventSubscribe("Crop2MLProcess")]
    public void CalculateModel(object sender, EventArgs e)
    {
        if (clock.Today == clock.StartDate)
        {
            Init();
        }
        setExogenous();
        soiltempComponent.CalculateModel(s,s1, r, a, ex);
    }

}