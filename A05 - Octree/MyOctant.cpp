#include "MyOctant.h"
using namespace Simplex;


MyOctant::MyOctant(uint a_nMaxLevel, uint a_nIdealEntityCount)
{
	Init();
	m_uIdealEntityCount = a_nIdealEntityCount;
	m_uMaxLevel = a_nMaxLevel;
}
MyOctant::MyOctant(vector3 a_v3Center, float a_fSize)
{
	Init();
	m_v3Center = a_v3Center;
	m_fSize = a_fSize;
}
MyOctant::MyOctant(MyOctant const& other)
{
	m_uOctantCount = other.m_uOctantCount;
	m_uMaxLevel = other.m_uMaxLevel;
	m_uIdealEntityCount = other.m_uIdealEntityCount;
	m_uID = other.m_uID;
	m_uLevel = other.m_uLevel;
	m_uChildren = m_uChildren;
	m_fSize = other.m_fSize;
	m_pMeshMngr = other.m_pMeshMngr;
	m_pEntityMngr = other.m_pEntityMngr;
	m_v3Center = other.m_v3Center;
	m_v3Min = other.m_v3Min;
	m_v3Max = other.m_v3Max;
	m_pParent = other.m_pParent;
	m_pChild[8] = other.m_pChild[8];
	m_EntityList = other.m_EntityList;
	m_pRoot = other.m_pRoot;
	m_lChild = other.m_lChild;
}
MyOctant&::MyOctant::operator=(MyOctant const& other)
{
	if (this != &other)
	{
		Release();
		Init();
		MyOctant temp(other);
		Swap(temp);
	}
	return *this;
}
MyOctant::~MyOctant() { Release(); }

void Simplex::MyOctant::Swap(MyOctant & other)
{
	std::swap(m_uOctantCount,other.m_uOctantCount);
	std::swap(m_uMaxLevel,other.m_uMaxLevel);
	std::swap(m_uIdealEntityCount,other.m_uIdealEntityCount);
	std::swap(m_uID,other.m_uID);
	std::swap(m_uLevel, other.m_uLevel);
	std::swap(m_uChildren, m_uChildren);
	std::swap(m_fSize, other.m_fSize);
	std::swap(m_pMeshMngr, other.m_pMeshMngr);
	std::swap(m_pEntityMngr, other.m_pEntityMngr);
	std::swap(m_v3Center, other.m_v3Center);
	std::swap(m_v3Min, other.m_v3Min);
	std::swap(m_v3Max, other.m_v3Max);
	std::swap(m_pParent, other.m_pParent);
	std::swap(m_pChild[8], other.m_pChild[8]);
	std::swap(m_EntityList, other.m_EntityList);
	std::swap(m_pRoot, other.m_pRoot);
	std::swap(m_lChild, other.m_lChild);
}

float Simplex::MyOctant::GetSize(void){return m_fSize;}

vector3 Simplex::MyOctant::GetCenterGlobal(void){return m_v3Center;}

vector3 Simplex::MyOctant::GetMinGlobal(void){return m_v3Min;}

vector3 Simplex::MyOctant::GetMaxGlobal(void){return m_v3Max;}

bool Simplex::MyOctant::IsColliding(uint a_uRBIndex)
{
	for (int i = 0; i < m_EntityList.size(); i++)
	{
		m_pEntityMngr->GetEntity(m_EntityList[i])->IsColliding(m_pEntityMngr->GetEntity(a_uRBIndex));
	}
}

void Simplex::MyOctant::Display(uint a_nIndex, vector3 a_v3Color)
{
	MyEntity* temp = m_pEntityMngr->GetEntity(a_nIndex);
	m_pMeshMngr->AddWireCubeToRenderList(temp->GetModelMatrix(),a_v3Color,RENDER_WIRE);
}

void Simplex::MyOctant::Display(vector3 a_v3Color)
{
	for (int i = 0; i < m_EntityList.size(); i++)
	{
		Display(m_EntityList[i], a_v3Color);
	}
}

void Simplex::MyOctant::DisplayLeafs(vector3 a_v3Color)
{
	for (int i = 0; i < m_lChild.size(); i++)
	{
		Display(i,a_v3Color);
	}
}

void Simplex::MyOctant::ClearEntityList(void){m_EntityList.clear();}

void Simplex::MyOctant::Subdivide(void)
{

}

MyOctant * Simplex::MyOctant::GetChild(uint a_nChild){return m_pChild[a_nChild];}

MyOctant * Simplex::MyOctant::GetParent(void){return m_pParent;}

bool Simplex::MyOctant::IsLeaf(void)
{
	if(this->GetParent())//if there is a parent, is a leaf
		return false;
	if (!this->GetParent())//if there is not a parent, is not a leaf
		return true;
}

bool Simplex::MyOctant::ContainsMoreThan(uint a_nEntities)
{
	return false;
}

void Simplex::MyOctant::KillBranches(void)
{
}

void Simplex::MyOctant::ConstructTree(uint a_nMaxLevel)
{
}

void Simplex::MyOctant::AssignIDtoEntity(void)
{
}

uint Simplex::MyOctant::GetOctantCount(void){return m_uOctantCount;}

void Simplex::MyOctant::Release(void)
{
	m_pMeshMngr = nullptr;
	ClearEntityList();
	m_lChild.clear();
	m_pChild[8] = nullptr;
	m_pEntityMngr = nullptr;
	m_pParent = nullptr;
	m_pRoot = nullptr;
}

void Simplex::MyOctant::Init(void)
{
	m_uOctantCount = 0;
	m_uMaxLevel = 0;
	m_uIdealEntityCount = 0;

	m_uID = 0;
	m_uLevel = 0;
	m_uChildren = 0;

	m_fSize = 0.0f;

	m_pMeshMngr = MeshManager::GetInstance();
	m_pEntityMngr = MyEntityManager::GetInstance();

	m_v3Center = ZERO_V3;
	m_v3Min = ZERO_V3;
	m_v3Max = ZERO_V3;

	m_pParent = nullptr;
	m_pChild[8];

	m_pRoot = nullptr;
}

void Simplex::MyOctant::ConstructList(void)
{
}

