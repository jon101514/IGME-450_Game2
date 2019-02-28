#include "AppClass.h"
void Application::InitVariables(void)
{
	m_pCameraMngr->SetPositionTargetAndUpward(vector3(0.0f,0.0f,7.0f),vector3(0.0f,0.0f,0.0f),vector3(0.0f,1.0f,0.0f));
	//init the mesh
	m_pMesh = new MyMesh();
	//m_pMesh->GenerateCube(1.0f, C_WHITE);
	m_pMesh->GenerateCone(2.0f, 5.0f, 3, C_WHITE);
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

	
	float fAspect = m_pSystem->GetWindowWidth()/static_cast<float>(m_pSystem->GetWindowHeight());
	float fNear = 0.01f;
	float fFar = 6.5f;
	matrix4 m4Projection = glm::perspective(m_fFOV,fAspect,fNear,fFar);
	
	vector3 v3Target=m_v3Eye+vector3(0.0f,0.0f,-1.0f);//cam look at
	vector3 v3Upward=vector3(0.0f,1.0f,0.0f);//what up means
	matrix4 m4View = glm::lookAt(m_v3Eye,v3Target,v3Upward);

	m4Projection = glm::ortho(-10.0f,10.0f,-10.0f,10.0f,fNear,fFar);

	matrix4 m4OrientX = glm::rotate(IDENTITY_M4, glm::radians(m_v3Rotation.x), vector3(1.0f, 0.0f, 0.0f));
	matrix4 m4OrientY = glm::rotate(IDENTITY_M4, glm::radians(m_v3Rotation.y), vector3(0.0f, 1.0f, 0.0f));
	matrix4 m4OrientZ = glm::rotate(IDENTITY_M4, glm::radians(m_v3Rotation.z), vector3(0.0f, 0.0f, 1.0f));
	matrix4 m4Orientation = m4OrientZ* m4OrientY*m4OrientX;
	//m_m4Model = glm::rotate(IDENTITY_M4, glm::radians(m_v3Rotation.x), vector3(1.0f, 0.0f, 0.0f));
	//m_m4Model = glm::rotate(m_m4Model, glm::radians(m_v3Rotation.y), vector3(0.0f, 1.0f, 0.0f));
	//m_m4Model = glm::rotate(m_m4Model, glm::radians(m_v3Rotation.z), vector3(0.0f, 0.0f, 1.0f));
	//m_pMesh->Render(m4Projection, m4View, ToMatrix4(m_m4Model));
	
	
	m_pMesh->Render(m4Projection, m4View, m4Orientation);
	
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
}
void Application::Release(void)
{
	SafeDelete(m_pMesh);

	//release GUI
	ShutdownGUI();
}