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
namespace Models.Crop2ML;

/// <summary>
///  This class encapsulates the Model_SoilTempCampbellComponent
/// </summary>
[Serializable]
[PresenterName("UserInterface.Presenters.PropertyPresenter")]
[ViewName("UserInterface.Views.PropertyView")]
[ValidParent(ParentType = typeof(Zone))]
[ValidParent(ParentType = typeof(Factor))]
public class Model_SoilTempCampbellWrapper :  Model, ISoilTemperature
{
    [Link] Clock clock = null;
    [Link] Weather weather = null;
    [Link] Physical physical = null;
    [Link] Organic organic = null;
    [Link] Water water = null;
    [Link] WaterBalance waterBalance = null;
    [Link] MicroClimate microClimate = null;
    [Link] Simulation simulation = null;

    private Model_SoilTempCampbellState s;
    private Model_SoilTempCampbellState s1;
    private Model_SoilTempCampbellRate r;
    private Model_SoilTempCampbellAuxiliary a;
    private Model_SoilTempCampbellExogenous ex;
    private Model_SoilTempCampbellComponent model_soiltempcampbellComponent;

    /// <summary>
    ///
    /// </summary>
    public event EventHandler SoilTemperatureChanged;


    /// <summary>
    ///  The constructor of the Wrapper of the Model_SoilTempCampbellComponent
    /// </summary>
    public Model_SoilTempCampbellWrapper()
    {
        s = new Model_SoilTempCampbellState();
        s1 = new Model_SoilTempCampbellState();
        r = new Model_SoilTempCampbellRate();
        a = new Model_SoilTempCampbellAuxiliary();
        ex = new Model_SoilTempCampbellExogenous();
        model_soiltempcampbellComponent = new Model_SoilTempCampbellComponent();
    }

    /// <summary>
    ///  The get method of the APSIM soil layer depths of layers output variable
    /// </summary>
    [Description("APSIM soil layer depths of layers")]
    [Units("mm")]
    public List<double> THICKApsim{ get { return s.THICKApsim;}}


    /// <summary>
    ///  The get method of the Apsim node depths output variable
    /// </summary>
    [Description("Apsim node depths")]
    [Units("m")]
    public List<double> DEPTHApsim{ get { return s.DEPTHApsim;}}


    /// <summary>
    ///  The get method of the Apsim bd (soil bulk density) output variable
    /// </summary>
    [Description("Apsim bd (soil bulk density)")]
    [Units("g/cm3             uri :")]
    public List<double> BDApsim{ get { return s.BDApsim;}}


    /// <summary>
    ///  The get method of the Apsim proportion of CLAY in each layer of profile output variable
    /// </summary>
    [Description("Apsim proportion of CLAY in each layer of profile")]
    [Units("")]
    public List<double> CLAYApsim{ get { return s.CLAYApsim;}}


    /// <summary>
    ///  The get method of the Apsim volumetric water content output variable
    /// </summary>
    [Description("Apsim volumetric water content")]
    [Units("cc water / cc soil")]
    public List<double> SWApsim{ get { return s.SWApsim;}}


    /// <summary>
    ///  The get method of the Temperature at end of last time-step within a day - midnight in layers output variable
    /// </summary>
    [Description("Temperature at end of last time-step within a day - midnight in layers")]
    [Units("degC")]
    public List<double> soilTemp{ get { return s.soilTemp;}}


    /// <summary>
    ///  The get method of the Soil temperature at the end of one iteration output variable
    /// </summary>
    [Description("Soil temperature at the end of one iteration")]
    [Units("degC")]
    public List<double> newTemperature{ get { return s.newTemperature;}}


    /// <summary>
    ///  The get method of the Minimum soil temperature in layers output variable
    /// </summary>
    [Description("Minimum soil temperature in layers")]
    [Units("degC")]
    public double[] MinimumSoilTemperature
    {
        get
        {
            int numLayers = model_soiltempcampbellComponent.NLAYR;
            int topsoilNode = 2;
            double[] result = new double[numLayers];
            Array.ConstrainedCopy(s.minSoilTemp.ToArray(), topsoilNode, result, 0, numLayers);
            return result;
        }
    }


    /// <summary>
    ///  The get method of the Maximum soil temperature in layers output variable
    /// </summary>
    [Description("Maximum soil temperature in layers")]
    [Units("degC")]
    public double[] MaximumSoilTemperature
    {
        get
        {
            int numLayers = model_soiltempcampbellComponent.NLAYR;
            int topsoilNode = 2;
            double[] result = new double[numLayers];
            Array.ConstrainedCopy(s.maxSoilTemp.ToArray(), topsoilNode, result, 0, numLayers);
            return result;
        }
    }


    /// <summary>Temperature of soil layers averaged over a day</summary>
    /// <remarks>Mandatory for ISoilTemperature interface. For now, just return average daily values - CHECK</remarks>
    [Units("oC")]
    public double[] Value { get { return AverageSoilTemperature; } }

    /// <summary>
    ///  The get method of the Temperature averaged over all time-steps within a day in layers. output variable
    /// </summary>
    [Description("Temperature averaged over all time-steps within a day in layers.")]
    [Units("degC")]
    public double[] AverageSoilTemperature
    {
        get
        {
            int numLayers = model_soiltempcampbellComponent.NLAYR;
            int topsoilNode = 2;
            double[] result = new double[numLayers];
            Array.ConstrainedCopy(s.aveSoilTemp.ToArray(), topsoilNode, result, 0, numLayers);
            return result;
        }
    }


    /// <summary>
    ///  The get method of the Temperature  in the morning in layers. output variable
    /// </summary>
    [Description("Temperature  in the morning in layers.")]
    [Units("degC")]
    public List<double> morningSoilTemp{ get { return s.morningSoilTemp;}}


    /// <summary>
    ///  The get method of the thermal conductivity coeff in layers output variable
    /// </summary>
    [Description("thermal conductivity coeff in layers")]
    [Units("(W/m2/K)")]
    public List<double> thermalCondPar1{ get { return s.thermalCondPar1;}}


    /// <summary>
    ///  The get method of the thermal conductivity coeff in layers output variable
    /// </summary>
    [Description("thermal conductivity coeff in layers")]
    [Units("(W/m2/K)")]
    public List<double> thermalCondPar2{ get { return s.thermalCondPar2;}}


    /// <summary>
    ///  The get method of the thermal conductivity coeff in layers output variable
    /// </summary>
    [Description("thermal conductivity coeff in layers")]
    [Units("(W/m2/K)")]
    public List<double> thermalCondPar3{ get { return s.thermalCondPar3;}}


    /// <summary>
    ///  The get method of the thermal conductivity coeff in layers output variable
    /// </summary>
    [Description("thermal conductivity coeff in layers")]
    [Units("(W/m2/K)")]
    public List<double> thermalCondPar4{ get { return s.thermalCondPar4;}}


    /// <summary>
    ///  The get method of the thermal conductivity in layers output variable
    /// </summary>
    [Description("thermal conductivity in layers")]
    [Units("(W/m2/K)")]
    public List<double> thermalConductivity{ get { return s.thermalConductivity;}}


    /// <summary>
    ///  The get method of the Thermal conductance between layers output variable
    /// </summary>
    [Description("Thermal conductance between layers")]
    [Units("(W/m2/K)")]
    public List<double> thermalConductance{ get { return s.thermalConductance;}}


    /// <summary>
    ///  The get method of the Heat storage between layers (internal) output variable
    /// </summary>
    [Description("Heat storage between layers (internal)")]
    [Units("J/s/K")]
    public List<double> heatStorage{ get { return s.heatStorage;}}


    /// <summary>
    ///  The get method of the Volumetric specific heat over the soil profile output variable
    /// </summary>
    [Description("Volumetric specific heat over the soil profile")]
    [Units("J/K/m3")]
    public List<double> volSpecHeatSoil{ get { return s.volSpecHeatSoil;}}


    /// <summary>
    ///  The get method of the Air max temperature from previous day output variable
    /// </summary>
    [Description("Air max temperature from previous day")]
    [Units("")]
    public double maxTempYesterday{ get { return s.maxTempYesterday;}}


    /// <summary>
    ///  The get method of the Air min temperature from previous day output variable
    /// </summary>
    [Description("Air min temperature from previous day")]
    [Units("")]
    public double minTempYesterday{ get { return s.minTempYesterday;}}


    /// <summary>
    ///  The get method of the Apsim volumetric fraction of organic matter in the soil output variable
    /// </summary>
    [Description("Apsim volumetric fraction of organic matter in the soil")]
    [Units("")]
    public List<double> SLCARBApsim{ get { return s.SLCARBApsim;}}


    /// <summary>
    ///  The get method of the Apsim volumetric fraction of rocks in the soil output variable
    /// </summary>
    [Description("Apsim volumetric fraction of rocks in the soil")]
    [Units("")]
    public List<double> SLROCKApsim{ get { return s.SLROCKApsim;}}


    /// <summary>
    ///  The get method of the Apsim volumetric fraction of silt in the soil output variable
    /// </summary>
    [Description("Apsim volumetric fraction of silt in the soil")]
    [Units("")]
    public List<double> SLSILTApsim{ get { return s.SLSILTApsim;}}


    /// <summary>
    ///  The get method of the Apsim volumetric fraction of sand in the soil output variable
    /// </summary>
    [Description("Apsim volumetric fraction of sand in the soil")]
    [Units("")]
    public List<double> SLSANDApsim{ get { return s.SLSANDApsim;}}


    /// <summary>
    ///  The get method of the Boundary layer conductance output variable
    /// </summary>
    [Description("Boundary layer conductance")]
    [Units("K/W")]
    public double _boundaryLayerConductance{ get { return s._boundaryLayerConductance;}}

    /// <summary>
    ///
    /// </summary>
    public double AverageSoilSurfaceTemperature => s.aveSoilTemp.ToArray()[1];

    /// <summary>
    ///
    /// </summary>
    public double MinimumSoilSurfaceTemperature => s.minSoilTemp.ToArray()[1];

    /// <summary>
    ///
    /// </summary>
    public double MaximumSoilSurfaceTemperature => s.maxSoilTemp.ToArray()[1];


    /// <summary>
    ///  The Constructor copy of the wrapper of the Model_SoilTempCampbellComponent
    /// </summary>
    /// <param name="toCopy"></param>
    /// <param name="copyAll"></param>
    public Model_SoilTempCampbellWrapper(Model_SoilTempCampbellWrapper toCopy, bool copyAll)
    {
        s = (toCopy.s != null) ? new Model_SoilTempCampbellState(toCopy.s, copyAll) : null;
        r = (toCopy.r != null) ? new Model_SoilTempCampbellRate(toCopy.r, copyAll) : null;
        a = (toCopy.a != null) ? new Model_SoilTempCampbellAuxiliary(toCopy.a, copyAll) : null;
        ex = (toCopy.ex != null) ? new Model_SoilTempCampbellExogenous(toCopy.ex, copyAll) : null;
        if (copyAll)
        {
            model_soiltempcampbellComponent = (toCopy.model_soiltempcampbellComponent != null) ? new Model_SoilTempCampbellComponent(toCopy.model_soiltempcampbellComponent) : null;
        }
    }

    /// <summary>
    ///  The Initialization method of the wrapper of the Model_SoilTempCampbellComponent
    /// </summary>
    public void Init(){
        setExogenous();
        loadParameters();
        model_soiltempcampbellComponent.Init(s, s1, r, a, ex);
    }

    /// <summary>
    ///  Load parameters of the wrapper of the Model_SoilTempCampbellComponent
    /// </summary>
    private void loadParameters()
    {
        model_soiltempcampbellComponent.NLAYR = physical.Thickness.Length;
        model_soiltempcampbellComponent.THICK =  physical.Thickness;
        model_soiltempcampbellComponent.BD = physical.BD;
        model_soiltempcampbellComponent.SLCARB = organic.Carbon;
        model_soiltempcampbellComponent.CLAY = physical.ParticleSizeClay;
        model_soiltempcampbellComponent.SLROCK = physical.Rocks;
        model_soiltempcampbellComponent.SLSILT = physical.ParticleSizeSilt;
        model_soiltempcampbellComponent.SLSAND = physical.ParticleSizeSand;
        model_soiltempcampbellComponent.SW = water.Volumetric;
        model_soiltempcampbellComponent.CONSTANT_TEMPdepth = 10000;
        model_soiltempcampbellComponent.TAV = weather.Tav;
        model_soiltempcampbellComponent.TAMP = weather.Amp;
        model_soiltempcampbellComponent.XLAT = weather.Latitude;
        model_soiltempcampbellComponent.SALB = waterBalance.Salb;
        model_soiltempcampbellComponent.instrumentHeight = 0;
        model_soiltempcampbellComponent.boundaryLayerConductanceSource = "calc";
        model_soiltempcampbellComponent.netRadiationSource = "calc";
    }

    /// <summary>
    ///  Set exogenous variables of the wrapper of the Model_SoilTempCampbellComponent
    /// </summary>
    private void setExogenous()
    {
        ex.T2M = weather.MeanT;
        ex.TMAX = weather.MaxT;
        ex.TMIN = weather.MinT;
        ex.DOY = clock.Today.DayOfYear;
        ex.airPressure = weather.AirPressure;
        ex.canopyHeight = microClimate.CanopyHeight;
        ex.SRAD = weather.Radn;
        //ex.ESP = waterBalance.Eos;
        ex.ESP = (double)simulation.Get("[ReadWTHFile].Script.PotEvaporation"); 
        //ex.ES = waterBalance.Es;
        ex.ES = (double)simulation.Get("[ReadWTHFile].Script.ActualEvaporation");
        //ex.EOAD = waterBalance.Eo;
        ex.EOAD = (double)simulation.Get("[ReadWTHFile].Script.PotEvapotranspiration");
        ex.windSpeed = weather.Wind;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    [EventSubscribe("Crop2MLProcess")]
    public void CalculateModel(object sender, EventArgs e)
    {
        if (clock.Today == clock.StartDate)
        {
            Init();
        }
        setExogenous();
        model_soiltempcampbellComponent.CalculateModel(s,s1, r, a, ex);
        SoilTemperatureChanged?.Invoke(this, EventArgs.Empty);
    }

}