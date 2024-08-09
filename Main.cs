using ModFrameworkNs.Publish;
using ProjectSchoolModNs.Publish;
using AcademySmith;

//标记脚步入口
[assembly:ModEntry]
namespace AcademySmith
{
    // 必须要有一个模组功能类，负责注册各种游戏功能
    // There must be a module feature class, which is responsible for registering various game features 
    public class Main : BootableModFunctionBase
    {
        // 游戏上下文接口
        // Game context interface
        M_IGameContext gameContext;

        protected override void OnInit()
        {
            // 从模组上下文中获取游戏上下文接口
            // Gets the game context interface from the module context
            context.RunnerContext.TryGetContext(out gameContext);
        }

        protected override void OnEnable()
        {
            // 注册自定义的模块
            // Register custom modules
            gameContext.ModuleContext.RegisterGameModule<Modules>(context);
        }
        protected override void OnDisable()
        {
            // 注销自定义模块
            // Deregister the custom module
            gameContext.ModuleContext.UnregisterGameModule<Modules>(context);
        }

        protected override void OnDispose()
        {
        }
    }
}
