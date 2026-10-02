using Zenject;

namespace WhiteKNight
{
    public class CharacterSpawnUseCase : ICharacterSpawnUseCase
    {
        [Inject(Id = "PlayerModel")] private readonly ICharacterModel playCharacterModel;
        [Inject(Id = "NpcModel")] private readonly ICharacterModel npcCharacterModel;

        public CharacterSpawnUseCase([Inject(Id = "PlayerModel")] ICharacterModel playCharacterModel,
              [Inject(Id = "NpcModel")] ICharacterModel npcCharacterModel)
        {
            this.playCharacterModel = playCharacterModel;
            this.npcCharacterModel = npcCharacterModel;
        }

        public ICharacterModel GetPlayCharacterModel()
        {
            return this.playCharacterModel;
        }

        public ICharacterModel GetNpcCharacterModel()
        {
            return this.npcCharacterModel;
        }
      
    }
}

