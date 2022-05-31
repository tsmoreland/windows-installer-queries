# CMakeList.txt : CMake project for installed_version_finder, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.8)

find_package(glog 0.5.0 REQUIRED)

set (source_files "main.cpp" "ProductInfo.ixx" "Guid.ixx")

# Add source to this project's executable.
add_executable (installed_version_finder ${source_files})
target_link_libraries (installed_version_finder glog::glog)

set_property(TARGET installed_version_finder PROPERTY CXX_STANDARD 20)

if (CMAKE_VERSION VERSION_GREATER 3.12)
  set_property(TARGET installed_version_finder PROPERTY CXX_STANDARD 20)
endif()

# TODO: Add tests and install targets if needed.
install(TARGETS installed_version_finder
  EXPORT installed_version_finder
  RUNTIME DESTINATION bin
)
