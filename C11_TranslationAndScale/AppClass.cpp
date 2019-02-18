#include "AppClass.h"
void Application::InitVariables(void)
{
	//init the mesh
	m_pMesh = new MyMesh();
	m_pMesh2 = new MyMesh();
	//m_pMesh->GenerateCube(1.0f, C_WHITE);
	m_pMesh->GenerateCone(1.0f, 3.0f,5, C_WHITE);
	//m_pMesh2->GenerateTorus(1.0f, 1.0f,5, 5,C_WHITE);
}
void Application::Update(void)
{
	//Update the system so it knows how much time has passed since the last call
	m_pSystem->Update();

	//Is the arcball active?
	ArcBall();

	//Is the first person camera active?
	CameraRotation();
}
void Application::Display(void)
{
	// Clear the screen
	ClearScreen();
	static float fRot = 0.0f;

	matrix4 m4View = m_pCameraMngr->GetViewMatrix();
	matrix4 m4Projection = m_pCameraMngr->GetProjectionMatrix();
	
	//matrix4 m4Scale = glm::scale(IDENTITY_M4, vector3(2.0f,2.0f,2.0f));
	static float value = 0.0f;
	//matrix4 m4Translate = glm::translate(IDENTITY_M4, vector3(value, 2.0f, 3.0f));
	



	//matrix4 m4Model = m4Translate * m4Scale;
	//matrix4 m4Model = m4Scale * m4Translate;
	matrix4 m4Model = glm::rotate(IDENTITY_M4,glm::radians(90.0f),vector3(0.0f,0.0f,fRot));
	
	//m4Model=glm::translate(vector3(1.0f,0.0f,0.0f));
	vector4 v4=m4Model[3];
	/*
	static DWORD startTime = GetTickCount();
	DWORD currentTime = GetTickCount();
	DWORD deltaTime = currentTime - startTime;
	std::cout << deltaTime / 1000.0f << std::endl;
	*/
	uint uClock = m_pSystem->GenClock();
	float fDeltaTime=m_pSystem->GetDeltaTime(uClock);
	std::cout << fDeltaTime << std::endl;

	//m4Model[3][0] = 1.0f;
	//v4.x = 1.0f;
	//for (size_t i = 0; i < 3; i++)
		//m4Model[i][i] = 5.0f;
	//m4Scale= glm::scale(IDENTITY_M4, vector3(1.0f));
	//m4Model = m4Translate*m4Scale;
	//m4Model = m4Scale * m4Translate;
	
	m_pMesh->Render(m4Projection, m4View, m4Model);
	fRot += 1.0f;
	// draw a skybox
	m_pMeshMngr->AddSkyboxToRenderList();
	
	//render list call
	m_uRenderCallCount = m_pMeshMngr->Render();

	//clear the render list
	m_pMeshMngr->ClearRenderList();
	
	//draw gui
	DrawGUI();
	
	//end the current frame (internally swaps the front and back buffers)
	m_pWindow->display();
	value += 0.01f;
}
void Application::Release(void)
{
	SafeDelete(m_pMesh);
	SafeDelete(m_pMesh2);
	//release GUI
	ShutdownGUI();
}